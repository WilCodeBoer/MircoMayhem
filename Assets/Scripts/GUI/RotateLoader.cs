using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoader : MonoBehaviour {
    Vector3 rotationEuler;

    void Update () {
        rotationEuler -= Vector3.forward * 60 * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotationEuler);
    }
}
