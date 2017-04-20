using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Makes use of rigidbody and transforms
public class CarController : MonoBehaviour {
	
	public float mass, drag, customSpeed, speed, distanceTraveled, 
		rotateSpeed = 10, 
		rotateAngle = 180, 
		customDrag = 50, 
		timer = 3;
    
	public bool respawnNow = false, 
		isGrounded = true, 
		isFlipped = false, 
		entered = false, 
		isFalling = false,
		flattened = false, 
		broken = false;

	public Vector3 moveDirection = Vector3.zero, 
		rotateDirection = Vector3.zero, 
		reverseDirection = Vector3.zero,
		totalDistance = Vector3.zero, 
		betweenCheckPoints;

	public List<GameObject> wheels = new List<GameObject> ();
	public GameObject tiltMessage;
	public GameObject carPrefab;
	public Material highlight;
	public Material normal;
	public Material driftMark;
	public Camera playerCamera;
    public Rigidbody ownRB;

	GameObject self;
	ConstantForce cf;
	Quaternion spawnRotation;
	Vector3 spawnLocation;
	Vector3 portalLocation;
	Vector3 scale;
	Vector3 childScale = new Vector3 (0.2985f, 0.2985f, 0.2985f);
	List<Transform> parts = new List<Transform>();
	List<Vector3> checkpoints = new List<Vector3>();

	public enum CurrentInput{ Keyboard, Controller1, Controller2, KeyboardController };
	public CurrentInput Controls;

	public enum Gears{ First, Second, Third };
	public Gears currentGear;
  
    public Rounds roundsRef;

    void Start () {
		cf = gameObject.GetComponent<ConstantForce> ();
		scale = transform.localScale;
        self = this.gameObject;
        portalLocation = FindObjectOfType<portalEnd>().gameObject.transform.position;
		ownRB = self.GetComponent<Rigidbody> ();

		foreach (Transform wheel in self.GetComponentsInChildren<Transform>()) {
			if (wheel.name.ToLower ().Contains ("wheel")) {
				wheels.Add (wheel.gameObject);
			}
		}
	}

	void FixedUpdate(){
		CheckInput ();
	}

	void Update() {
		//Check wether we are grounded and visible. Doing this in an Couroutine because update does it way faster then it needs to be checked. once every 0.1 second is more then fast enough
		StartCoroutine(CheckGrounded());
		StartCoroutine (CheckVisible());

		if (respawnNow) {
			Respawn();
		}

		if (tiltMessage == null) {
			if (gameObject.GetComponentInChildren<TextMesh> () != null) {
				tiltMessage = gameObject.GetComponentInChildren<TextMesh> ().gameObject;
			}
		}
		//Switch for the controls
		switch (Controls) {
			case CurrentInput.Keyboard:
				moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical") * speed);
				rotateDirection = new Vector3(0, Input.GetAxis("Horizontal") * rotateAngle, 0);
					if(tiltMessage!= null)tiltMessage.GetComponent<TextMesh>().text = "Press Q to tilt!";
				break;
			case CurrentInput.Controller1:
				reverseDirection = new Vector3 (0, 0, -Input.GetAxis ("leftTrigger1") * speed);
				moveDirection = new Vector3 (0, 0, Input.GetAxis ("rightTrigger1") * speed);
				rotateDirection = new Vector3 (0, Input.GetAxis ("Steering1") * rotateAngle, 0);
					if(tiltMessage!= null)tiltMessage.GetComponent<TextMesh>().text = "Press Y to tilt!";
				break;
			case CurrentInput.Controller2:
				reverseDirection = new Vector3(0, 0, -Input.GetAxis("leftTrigger2") * speed);
				moveDirection = new Vector3(0, 0, Input.GetAxis("rightTrigger2") * speed);
				rotateDirection = new Vector3(0, Input.GetAxis("Steering2") * rotateAngle, 0);
					if(tiltMessage!= null)tiltMessage.GetComponent<TextMesh>().text = "Press Y to tilt!";
				break;
		}
		 
		//If we are grounded we need normal mass and drag
		if (isGrounded && !Input.GetButton("Handbrake1") && !flattened) {
			ownRB.mass = mass;
			ownRB.drag = drag;
			ownRB.angularDrag = 0.05f;
		} 
		//If we aren;t grounded we want to fall faster so a higher mass and drag is needed
		else if (!isGrounded && !Input.GetButton("Handbrake1")) {
			ownRB.mass = mass;
		}

		if (isFalling) {
			ownRB.drag = 0;
		}

		//Check if we got hit by a hammer, if so rescale the car
		if (flattened) {
			ownRB.mass = 2.5f;
			ownRB.drag = 0.5f;
			timer -= Time.deltaTime;
			if (timer <= 0) {
				ScaleCar ();
				timer = 3;
			}
		}
		if (broken) {
			timer -= Time.deltaTime;
			if (timer <= 0) {
				FixCar ();
				foreach (Transform part in parts) {
					if (part != null) {
						Destroy (part.gameObject);
					}
				}
				timer = 3;
			}
		}
	}

	//Handles keyboard input. Reccomend using controller!
    void InputActionKeyBoard() {
        transform.RotateAround(transform.position, rotateDirection, rotateSpeed * Time.deltaTime);

        if (isGrounded && !isFlipped) {
            ownRB.AddRelativeForce(reverseDirection * speed * Time.deltaTime);
            ownRB.AddRelativeForce(moveDirection * speed * Time.deltaTime);
        }

		if (Input.GetButton ("HandbrakeKey") && isGrounded) {
			ownRB.mass = 50;
			ownRB.drag = drag;
		}
		else if (Input.GetButton ("DriftKey") && isGrounded) {
			EnableDriftMarks ();	
			rotateAngle = 360;
			rotateSpeed = 175;
		}
        else if (Input.GetButton("TiltKey")) {
            if (isFlipped && isGrounded) {
                Tilt();
            }
        } else {
			DisableDriftMarks ();
			if (!flattened) {
				rotateAngle = 180;
				rotateSpeed = 100;
				ownRB.drag = drag;
				ownRB.mass = mass;
			}
        }

    }

	//Handles controller input. Int is player number
    void InputActionController(int player) {
		
        transform.RotateAround(transform.position, rotateDirection, rotateSpeed * Time.deltaTime);
        if (isGrounded && !isFlipped) {
			ownRB.AddRelativeForce(reverseDirection * speed * Time.deltaTime);
			ownRB.AddRelativeForce(moveDirection * speed * Time.deltaTime);
        }

        if (Input.GetButton("Handbrake" + player) && isGrounded) {
            ownRB.mass = 50;
			rotateAngle = 360;
            ownRB.drag = drag;
		}
		else if (Input.GetButton("Drift" + player) && isGrounded){
			EnableDriftMarks ();
			rotateAngle = 360;
			rotateSpeed = 175;
		}
		else if (Input.GetButton("Tilt" + player) && isFlipped && isGrounded) {
        	Tilt();
        }
        else {
			DisableDriftMarks ();
			if (!flattened) {
				rotateAngle = 180;
				rotateSpeed = 100;
				ownRB.drag = drag;
				ownRB.mass = mass;
			}
        }    
	}
		
	//Checks what input the player will use
	public void CheckInput() {
		switch (Controls) {
		case CurrentInput.Keyboard:
			InputActionKeyBoard ();
			break;
		case CurrentInput.Controller1:
			InputActionController (1);
			break;
		case CurrentInput.Controller2:
			InputActionController (2);
			break;
		}
	}

    //Currently useless but might have some use in the future!
	void SwitchGears() {
		switch (currentGear) {
			case Gears.First:
                speed = customSpeed;
				break;
			case Gears.Second:
				speed = 175;
				break;
			case Gears.Third:
				speed = 200;
				break;
			default:
				speed = 0;
				break;
		}
	}

	//Enter the portal to start a new round
    void EnterPortal() {
        transform.position = Vector3.Lerp(transform.position, portalLocation, speed * Time.deltaTime);
        roundsRef.rounds++;
        checkpoints.Clear();
    }

	void EnableDriftMarks(){
		foreach (GameObject wheel in wheels) {
			if (!wheel.GetComponent<TrailRenderer> ()) {
				TrailRenderer trail = wheel.AddComponent<TrailRenderer> ();
				trail.widthMultiplier = 0.04f;
				trail.time = 0.5f;
				trail.endColor = Color.black;
				trail.startColor = Color.gray;
				trail.material = driftMark;
			}
		}
	}

	void DisableDriftMarks(){
		foreach (GameObject wheel in wheels) {
			if (wheel.GetComponent<TrailRenderer> ()) {
				Destroy (wheel.GetComponent<TrailRenderer> ());
			}
		}
	}

    //Respaws back to current spawnLocation
    void Respawn() {
        transform.position = spawnLocation;
        transform.rotation = spawnRotation;
        isFalling = false;
        respawnNow = false;       
    }

    //put target back on it's wheels
    void Tilt() {
        Vector3 rot = new Vector3(0,transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rot), Time.deltaTime * speed);
    }

	//Fix the car once the sawblade has ripped it apart.
	void FixCar(){
		GameObject Car = GameObject.Instantiate (carPrefab);
		Car.transform.position = self.transform.position;
		Car.transform.rotation = self.transform.rotation;
		Car.transform.parent = self.transform;
		Car.transform.localScale = childScale;
		foreach (Transform wheel in Car.GetComponentsInChildren<Transform>()) {
			if (wheel.name.ToLower ().Contains ("wheel")) {
				wheels.Add (wheel.gameObject);
			}
		}
		Respawn ();
		broken = false;
	}


	//Rescale the car once. Required to set flatted to false or this keeps repeating itself
	void ScaleCar(){
		transform.localScale = scale;
		ownRB.mass = mass;
		ownRB.drag = drag;
		cf.force = new Vector3(0,-50,0);
		flattened = false;
	}

	//Check if we're visible or not. if not set highlight material
	IEnumerator CheckVisible(){
		yield return new WaitForSeconds (0.01f);
		RaycastHit hitInfo;
		float distance = Vector3.Distance (transform.position, playerCamera.transform.position);

		if(Physics.Raycast(transform.position, playerCamera.transform.position - transform.position, out hitInfo,distance)){
			if (hitInfo.collider.name.ToLower ().Contains ("book (rb)")) {
				if (transform.childCount >= 1) {
					self.transform.GetChild (0).GetChild (0).gameObject.GetComponent<MeshRenderer> ().material = highlight;
				}
			} else {
				if (transform.childCount >= 1) {
					self.transform.GetChild (0).GetChild (0).gameObject.GetComponent<MeshRenderer> ().material = normal;
				}
			} 
		}
	}

	//Check if we're on the ground or not
    IEnumerator CheckGrounded() {
        float maxDist = self.GetComponent<Collider>().bounds.extents.y;

        yield return new WaitForSeconds(0.1f);
        if (Physics.Raycast(transform.position, -Vector3.up, maxDist + 0.125f, 9)) {
            isGrounded = true;
        }
        else {
            isGrounded = false;
        }
        if (Vector3.Dot(transform.up, Vector3.down) > 0) {
            isFlipped = true;
			if (tiltMessage != null && isGrounded) {
				tiltMessage.gameObject.SetActive (true);
			}
        } else {
            isFlipped = false;
			if (tiltMessage != null) {
				tiltMessage.gameObject.SetActive (false);
			}
        }
    }

	void OnCollisionEnter(Collision collision) {
		switch (collision.collider.tag) {
		case "Floor":
			respawnNow = true;
			break;
		case "Cutter":
			ownRB.AddForce((Vector3.up * 100) + collision.contacts[0].point, ForceMode.Impulse);
			break;
		case "SawBlade":
			if (transform.childCount >= 1) {
				foreach (Transform part in transform.GetChild(0)) {
					if (part != null) {
						part.gameObject.AddComponent<Rigidbody> ();
						part.parent = null;
						part.gameObject.GetComponent<Rigidbody> ().AddForce (new Vector3 (Random.Range (1, 10), Random.Range (1, 10), Random.Range (1, 10)), ForceMode.Impulse);
						parts.Add (part);
					}
				}
				wheels.Clear ();
				Destroy (transform.GetChild (0).gameObject);
				isFalling = true;
				broken = true;
			} else {
				return;
			}
			break;
		case "Hammer":
			cf.force = new Vector3 (0, 0, 0);
			transform.localScale = new Vector3 (transform.localScale.x + 0.2f, 0.05f, transform.localScale.z + 0.2f);
			flattened = true;
			break;
		}
	}

	void OnTriggerEnter(Collider other) {
		switch (other.tag) {
			case "Portal":
				if (entered == false) {
					entered = true;
					EnterPortal();
				}
				break;
			case "PortalEnd":
				entered = false;
				break;
			case "FallTrigger":
				isFalling = true;
				break;
			case "CheckPoint":
				//Add position logic
				if (!checkpoints.Contains(other.gameObject.transform.position)) {
					checkpoints.Add(other.gameObject.transform.position);
					if (checkpoints.Count > 1) {
						distanceTraveled += checkpoints[checkpoints.Count - 1].magnitude;
					}
				}              
				break;
		}
	}

	//Sets spawn location at the edges of the spawn triggers
	void OnTriggerExit(Collider other) {
		switch (other.tag) {
			case "SpawnLocation":       
				spawnLocation = transform.position;
				spawnRotation = transform.localRotation;
				break;
		}
	}
}
