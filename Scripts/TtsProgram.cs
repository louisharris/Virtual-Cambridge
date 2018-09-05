using UnityEngine;
using VoiceRSS_SDK;

/// <summary>
/// Text to speech program
/// </summary>
class TtsProgram : MonoBehaviour{
    public void TextToSpeech(string text) {
        Debug.Log(text);
        var apiKey = "ADD VOICERSS KEY HERE";
        var isSSL = false;
        var lang = Languages.English_GreatBritain;

        var voiceParams = new VoiceParameters(text, lang) {
            AudioCodec = AudioCodec.WAV,
            AudioFormat = AudioFormat.Format_44KHZ.AF_44khz_16bit_stereo,
            IsBase64 = false,
            IsSsml = false,
            SpeedRate = 0
        };

        var voiceProvider = new VoiceProvider(apiKey, isSSL);
        var voice = voiceProvider.Speech<byte[]>(voiceParams);

        PrintReply.Audio = voice;
    }
}

