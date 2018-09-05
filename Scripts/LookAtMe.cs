using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Potential script to make model look at camera, didnt work well for when usinga skybox, but could be tweaked.
/// </summary>
public class LookAtMe : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var pos = Camera.main.transform.position - transform.position;
        pos[1] = pos[1]-2.1f;
        transform.rotation = Quaternion.LookRotation(pos);
    }
}
