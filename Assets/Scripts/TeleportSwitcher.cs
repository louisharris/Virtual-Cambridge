using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used when clicked on the teleport sphere above to change environments
/// Initial environments are Kings Walk, Webbs court and the Computer Lab
/// </summary>
public class TeleportSwitcher : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Switch() {
        bool atCl = false;
        if (RenderSettings.skybox.ToString().StartsWith("matstitch")) {
            atCl = true;
        }
        if (atCl) {
            Debug.Log("Telepored to Webbs");
            var s = new WebbsWalk();
            s.LoadEnvironmentConfiguration(1);
        }
        else {
            Debug.Log("Teleported to the Computer Labatory");
            var c = new ClWalk();
            c.LoadEnvironmentConfiguration(0);
        }
    }

    public void ChangeSkybox() {

        int dir = Convert.ToInt32(gameObject.name);
       

        if (RenderSettings.skybox.ToString().StartsWith("matstitch")) {
            var c = new ClWalk();
            c.ChangeSkybox(dir);
            
        }
        else {
            var s = new WebbsWalk();
            s.ChangeSkybox(dir);
        }
    }
}
