using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Net.Http;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// This class sends the speech byte data to Bing Speech API as well as LUIS to return a reply.
/// </summary>
public class Speech {

    private HttpWebRequest Request;
    Authentication Auth;
    
    class TextResult {
        public string Confidence { get; set; }
        public string Display { get; set; }
        public string Intent { get; set; }
    }

    public void getRequest() {

        if (Auth == null) Auth = new Authentication("ADD SPEECH TO TEXT KEY HERE");
        
        string requestUri = "https://speech.platform.bing.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US&format=detailed";

        string host = @"speech.platform.bing.com";
        string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";

        try {
            var token = Auth.GetAccessToken();
            Debug.Log("Token: {0}\n" + token);

            Request = null;
            Request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            Request.SendChunked = true;
            Request.Accept = @"application/json;text/xml";
            Request.Method = "POST";
            Request.ProtocolVersion = HttpVersion.Version11;
            Request.Host = host;
            Request.ContentType = contentType;
            Request.Headers["Authorization"] = "Bearer " + token;

        } catch(Exception ex) {
            Debug.Log(ex.ToString());
            Debug.Log(ex.Message);
        }
    }

    public void SendSpeechData(byte[] audio) {
        
        string responseString;

        getRequest();

        try {
            
            using (Stream requestStream = Request.GetRequestStream()) {

                // Read 1024 raw bytes from the input audio file.
                requestStream.Write(audio, 0, audio.Length);

                // Flush
                requestStream.Flush();
                requestStream.Close();
            }

            using (WebResponse response = Request.GetResponse()) {
                
                //Debug.Log(((HttpWebResponse)response).StatusCode);

                using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                    responseString = sr.ReadToEnd();
                }

                var SpeechLines = JObject.Parse(responseString);
                
                // get JSON result objects into a list
                var results = SpeechLines["NBest"].Children().ToList();
                var searchResults = new List<TextResult>();

                foreach (JToken result in results) {
                    // JToken.ToObject is a helper method that uses JsonSerializer internally
                    var searchResult = result.ToObject<TextResult>();
                    searchResults.Add(searchResult);
                }

                String retrievedText = searchResults[0].Display;

                Debug.Log("Sending to Luis");

                SentToLUISViaAPI(retrievedText);
            }

        }
        catch (Exception ex) {
            Debug.Log(ex.ToString());
            Debug.Log(ex.Message);
        }
    }

    public async void SentToLUISViaAPI(String query) {
  
        var client = new HttpClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);

        var luisAppId = "ADD LUIS APP ID HERE";
        var subscriptionKey = "ADD LUIS SUBSCIPTION KEY HERE";

        client.DefaultRequestHeaders.Add("Ocp-Apim-SubscriptionKey", subscriptionKey);

        var uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/6927daaf-0901-47f7-935f-0d3c6e039d51?subscription-key=072b4db924b74ae3a5de267cf9cab2a3&verbose=true&timezoneOffset=0&q="+query.Replace(" ", "%20");
        var response = await client.GetAsync(uri);

        var strResponseContent = await response.Content.ReadAsStringAsync();

        var SpeechLines = JObject.Parse(strResponseContent);

        // get JSON result objects into a list
        var result = SpeechLines["topScoringIntent"];

        var searchResult = result.ToObject<TextResult>();

        String retrievedIntent = searchResult.Intent;
        Debug.Log("query= " + query);
        Debug.Log("intent= " + retrievedIntent);
        PrintReply.ReadText = DivideText(query);
        PrintReply.SpeechText = query;
        PrintReply.Intent = retrievedIntent;
    }

    public static string DivideText(string query) {
        var charList = new List<char>();
        int count = 0;
        foreach (char c in query) {
            charList.Add(c);
            count++;
            if (count > 30 && c == ' ') {
                charList.Add(' ');
                charList.Add('\\');
                charList.Add('n');
                charList.Add(' ');
                count = 0;
            }
        }
        string res = Regex.Unescape(new string(charList.ToArray()));
        return res;
    }
}