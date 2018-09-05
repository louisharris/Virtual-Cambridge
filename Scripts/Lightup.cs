using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// Make directional arrows light up depending on the OnPointerEntry event of the corresponding panels
/// </summary>
public class Lightup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onMouseOver () {

        var fadedGreen = Resources.Load("mats/green bold") as Material;
        int dir = Convert.ToInt32(gameObject.name);
        
        //Getting list of all panels
        var objList = FindObjectsOfTypeAll<Transform>();
        for (int j = 0; j < objList.Count; j++) {
            var arrowPart = objList.ElementAt(j).gameObject;
            if (objList.ElementAt(j).parent && objList.ElementAt(j).parent.name.Equals("arrow"+dir)) {
                arrowPart.GetComponent<Renderer>().material = fadedGreen;
            }
        }
    }

    public void onMouseExit () {
        var boldGreen = Resources.Load("mats/green faded") as Material;
        int dir = Convert.ToInt32(gameObject.name);

        //Getting list of all panels
        var objList = FindObjectsOfTypeAll<Transform>();
        for (int j = 0; j < objList.Count; j++) {
            var arrowPart = objList.ElementAt(j).gameObject;
            if (objList.ElementAt(j).parent && objList.ElementAt(j).parent.name.Equals("arrow" + dir)) {
                arrowPart.GetComponent<Renderer>().material = boldGreen;
            }
        }
    }

    public static List<T> FindObjectsOfTypeAll<T>() {
        var results = new List<T>();
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            var s = SceneManager.GetSceneAt(i);
            if (s.isLoaded) {
                var allGameObjects = s.GetRootGameObjects();
                for (int j = 0; j < allGameObjects.Length; j++) {
                    var go = allGameObjects[j];
                    results.AddRange(go.GetComponentsInChildren<T>(true));
                }
            }
        }
        return results;
    }
}
