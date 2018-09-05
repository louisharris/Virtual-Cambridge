using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Self explanatory, to make the text follow the camera.
/// </summary>
public class FollowText : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
