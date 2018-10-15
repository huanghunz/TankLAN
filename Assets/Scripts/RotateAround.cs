using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour {

    public Transform RotationCenter;

	// Use this for initialization
	void Start () {
        if (RotationCenter == null) RotationCenter = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(RotationCenter.position, RotationCenter.transform.up, Time.deltaTime * 45f);
	}
}
