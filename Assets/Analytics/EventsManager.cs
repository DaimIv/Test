using Assets.Analytics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class EventsManager : MonoBehaviour
{
    public string serverURL = "";
    public List<POSTMessage> events;
    public int cooldownBeforeSend = 10;
    private  Timer sendTimer;

    private void Awake()
    {
       
        if(PlayerPrefs.HasKey("messages"))
        {
            events = JsonUtility.FromJson<List<POSTMessage>>(PlayerPrefs.GetString("messages"));
        }
        
        if (events == null)
        {
            events = new List<POSTMessage>();

        }
        sendTimer = new Timer(SendPOST, null, 0, cooldownBeforeSend*1000);
    }
    void SendPOST(object unused)
    {
        if (events.Count > 0)
        {
            print(events.Count.ToString()); 
            StartCoroutine(SendRequest(serverURL, UnityEngine.JsonUtility.ToJson(events)));
           
        }
    }
    void MessageSent()
    {
        events.Clear();
        SaveCurrentMessages();
    }

    void SaveCurrentMessages()
    {
        PlayerPrefs.SetString("messages",UnityEngine.JsonUtility.ToJson(events));
    }

    public void TrackEvent(POSTMessage message)
    {
        events.Add(message);
        SaveCurrentMessages();
    }
    private void Start()
    {
        //Example

        TrackEvent(new POSTMessage(EventsType.PayInGame, new object[2] { 500, "999222" }));

    }









    IEnumerator SendRequest(string url, string json)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        print("1");
        
        yield return request.SendWebRequest();


        if (request.downloadHandler.text == "200 OK")
        {
            MessageSent();
        }


    }
}
