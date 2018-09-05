//  Derived from Gregorio Zanon's script
//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734

using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
///  This class is similar to SavWav except instead of saving audio to a file it saves audio to a byte array
///  which can be returned and sent as audio data to the API
/// </summary>
public static class WavBuf {

    const int HEADER_SIZE = 44;

    public static byte[] Save(AudioClip clip) {
       
            var bytes = ConvertAndWrite(clip);

            bytes = WriteHeader(bytes, clip);
        

        return bytes;
    }

    public static AudioClip TrimSilence(AudioClip clip, float min) {
        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
    }

    public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz) {
        return TrimSilence(samples, min, channels, hz, false, false);
    }

    public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream) {
        int i;

        int cutCount = 0;
        for (i = 0; i < samples.Count; i++) {
            if (Mathf.Abs(samples[i]) > min) {
                cutCount++;
                if(cutCount > 20)
                break;
            }
        }

        samples.RemoveRange(0, i-20);

        for (i = samples.Count - 1; i > 0; i--) {
            if (Mathf.Abs(samples[i]) > min) {
                break;
            }
        }

        samples.RemoveRange(i, samples.Count - i);
        var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);
        clip.SetData(samples.ToArray(), 0);

        return clip;
    }

    static FileStream CreateEmpty(string filepath) {
        var fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++) // Preparing the header
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }

    static byte[] ConvertAndWrite(AudioClip clip) {

        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        // Converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        Byte[] bytesData = new Byte[samples.Length * 2];
        // BytesData array is twice the size of
        // DataSource array because a float converted in Int16 is 2 bytes.

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++) {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        return bytesData;
    }

    public static byte[] WriteHeader(byte[] bytes, AudioClip clip) {

        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        for(int i=0;i<4;i++) {
            bytes[i] = riff[i];
        }

        Byte[] chunkSize = BitConverter.GetBytes(bytes.Length - 8);
        for (int i = 4; i < 8; i++) {
            bytes[i] = chunkSize[i-4];
        }

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        for (int i = 8; i < 12; i++) {
            bytes[i] = wave[i-8];
        }

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        for (int i = 12; i < 16; i++) {
            bytes[i] = fmt[i-12];
        }

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        for (int i = 16; i < 20; i++) {
            bytes[i] = subChunk1[i-16];
        }

        UInt16 two = 2;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        for (int i = 20; i < 22; i++) {
            bytes[i] = audioFormat[i-20];
        }

        Byte[] numChannels = BitConverter.GetBytes(channels);
        for (int i = 22; i < 24; i++) {
            bytes[i] = numChannels[i-22];
        }

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        for (int i = 24; i < 28; i++) {
            bytes[i] = sampleRate[i-24];
        }

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        for (int i = 28; i < 32; i++) {
            bytes[i] = byteRate[i-28];
        }

        UInt16 blockAlign = (ushort)(channels * 2);
        for (int i = 32; i < 34; i++) {
            bytes[i] = BitConverter.GetBytes(blockAlign)[i-32];
        }

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        for (int i = 34; i < 36; i++) {
            bytes[i] = bitsPerSample[i-34];
        }

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        for (int i = 36; i < 40; i++) {
            bytes[i] = datastring[i-36];
        }

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        for (int i = 40; i < 44; i++) {
            bytes[i] = subChunk2[i-40];
        }
        return bytes;

    }
}