using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a base of communication from a running thread back to the main thread.
/// This is because you cannot return results directly from a thread.
/// </summary>
public class PrintReply : MonoBehaviour {

    public static string ReadText = "Click to talk";
    public static string SpeechText = "";
    public static string Intent = "none";
    public static byte[] Audio = null;

}
