/*using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This code will be commented out initially, only use when needing to batch create materials for each of the 360 images
/// Place all 360 images in unity Assets/resources folder then call this code
/// </summary>
public class Save : MonoBehaviour {    
    
    void Start() {

        // Loads all images and converts each into a skybox cubemap (example below,
        // change loop count + resource load name to match resource images
        // This example loads images named 360_0179_Stitch_XHC, 360_0180_Stitch_XHC, 360_0181_Stitch_XHC ... 360_0359_Stitch_XHC 
        for (int i = 179; i < 360; i += 180) {
            var img = (Cubemap)Resources.Load("images/360_0" + i + "_Stitch_XHC");
            var mat = new Material(Shader.Find("Skybox/Cubemap"));
            mat.SetTexture("_Tex", img);

            SaveObjectToFile(mat, "Assets/resources/mats/mat" + i + ".mat");
        }

    }


    public static void SaveObjectToFile(Object obj, string fileName) {
        AssetDatabase.CreateAsset(obj, fileName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // Update is called once per frame
    void Update() {

    }

}
*/