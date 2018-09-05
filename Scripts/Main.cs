using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>
/// Main thread for program, initialises other threads and retrieves results to output.
/// </summary>
public class Main : MonoBehaviour {

    float[] ClipSampleData = new float[1024];
    bool IsSpeaking = false;
    bool WasSpeaking = false;

    AudioSource AudioSourceSend;
    AudioSource AudioSourceReply;

    Speech S;

    float MinimumLevel;
    int PauseCount = 0;
    string Mic;
    byte[] PrevBytes;
    bool First = true;

    string InitialIntent = "none";

    // Use this for initialization
    void Start () {

        S = new Speech();

        var aSources = gameObject.GetComponents<AudioSource>();
        AudioSourceSend = aSources[0];
        AudioSourceReply = aSources[1];
        Mic = Microphone.devices[0].ToString();
        MinimumLevel = 6.0E-05f;

    }

    // Update is called once per frame
    void Update () {
        AudioSourceSend.GetSpectrumData(ClipSampleData, 0, FFTWindow.Rectangular);
        float currentAverageVolume = ClipSampleData.Average();
        if (currentAverageVolume > MinimumLevel) {
            IsSpeaking = true;
     
        }
        else if (IsSpeaking) {
            IsSpeaking = false;
            WasSpeaking = true;
            PauseCount = 1;
        } else {
            PauseCount++;
        }
         //Runs once stop speaking to send speech data
        if (PauseCount > 100 && WasSpeaking) {
            Debug.Log("Stopped speaking");
            PauseCount = 0;
            WasSpeaking = false;

            AudioSourceSend.clip = WavBuf.TrimSilence(AudioSourceSend.clip, MinimumLevel);
            var bytes = WavBuf.Save(AudioSourceSend.clip);

            new Thread(() => S.SendSpeechData(bytes)).Start();
        }

        //Runs text replacement checks and new audio checks
        bool changed = ReplaceText();
        PlayAudio();
        //Sends voice text to get a text-to-speech reply
        if (changed || First) {
            new Thread(() => SendTextData()).Start();
        }
        First = false;
    }

    //Sends text data from printReply to be processed in text-to-speech
    void SendTextData() {
        var tts = new TtsProgram();
        Debug.Log("About to speak the words: " + PrintReply.ReadText);
        tts.TextToSpeech(PrintReply.SpeechText);
    }

    //Replaces text above model to new updated text
    bool ReplaceText() {
        var TextObject = GameObject.Find("Reply Text");
        if (TextObject != null) {

            string initialText = TextObject.GetComponent<TextMesh>().text;

            //Can change intent replies depending on trained intent data in LUIS.
            switch(PrintReply.Intent) {
                case "booking":
                    if (!InitialIntent.Equals("booking")) {
                        PrintReply.ReadText = Speech.DivideText("We have punts available from 3pm onwards, would you like to book one for 3pm?");
                        PrintReply.SpeechText = "We have punts available from 3pm onwards, would you like to book one for 3pm?";
                        TextObject.GetComponent<TextMesh>().text = PrintReply.ReadText;
                        InitialIntent = "booking";
                        return true;
                    }
                    break;
                case "price":
                    if (!InitialIntent.Equals("price")) {
                        PrintReply.ReadText = Speech.DivideText("It will cost £15 per punt for one hour");
                        PrintReply.SpeechText = "It will cost £15 per punt for one hour";
                        TextObject.GetComponent<TextMesh>().text = PrintReply.ReadText;
                        InitialIntent = "price";

                        return true;
                    }
                        break;
                case "None":
                    if (!initialText.Equals(PrintReply.ReadText)) {
                        TextObject.GetComponent<TextMesh>().text = PrintReply.ReadText;
                        return true;
                    }
                    break;
                default:
                    break;
            }
        }
        return false;
    }
    
    
    //Creates and plays audio depending on the new updated audio bytes stores in audioSource
    void PlayAudio() {
        if (PrintReply.Audio != null && PrevBytes!=PrintReply.Audio) {
            var wav = new Wav(PrintReply.Audio);
            var audioClip = AudioClip.Create("testSound", wav.SampleCount, 1, wav.Frequency, false, false);
            audioClip.SetData(wav.LeftChannel, 0);
            
            AudioSourceReply.clip = audioClip;
            AudioSourceReply.Play();

            PrevBytes = PrintReply.Audio;
        }

    }

    //On click event, activates mic to talk, on second click plays stored audio file
    public void ActivateMic() {
        Debug.Log("Person clicked");

        AudioSourceSend.clip = Microphone.Start(Mic, true, 10, 44100);
        AudioSourceSend.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) {
        }
        AudioSourceSend.Play();
    }
}


