using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TensorFlow;

/// <summary>
/// Initial template for adding a tensorflow model to the project. TBD
/// </summary>
public class TrainedModel : MonoBehaviour {


	// Use this for initialization
	void Start () {

#if UNITY_ANDROID
        TensorFlowSharp.Android.NativeBinding.Init();
#endif

    }

    // Update is called once per frame
    void Update () {
		
	}
}
