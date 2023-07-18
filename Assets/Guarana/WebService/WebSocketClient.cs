using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

public class WebSocketClient
{
    private string handle;
    private string baseURL;
    private WebSocket ws;
    private bool isRunning;


    public WebSocketClient(string url, string handle, string baseURL)
    {
        this.handle = handle;
        this.baseURL = baseURL;
        isRunning = false;

        WebServiceUI.Log("Socket . ");
        ws = new WebSocket(url);

        ws.OnOpen += (sender, e) =>
        {
            isRunning = true;
            WebServiceUI.Log(". . connected");
        };

        ws.OnClose += (sender, e) =>
        {
            isRunning = false;
        };

        ws.OnMessage += (sender, e) =>
        {
            ReceiveMessage(e.Data);
        };

        ws.Connect();
    }


    private void ReceiveMessage(string data)
    {
        if (data.Contains("appId"))
        {
            ReceiveScene response = JsonUtility.FromJson<ReceiveScene>(data);
        }
        else if (data.Contains("eventType"))
        {
            ReceiveAction response = JsonUtility.FromJson<ReceiveAction>(data);
        }
    }


    public void SendMessage(APIMessage msg)
    {
        string data = JsonUtility.ToJson(msg);
        ws.Send(data);
    }


    public bool Running()
    {
        return isRunning;
    }


    public IEnumerator CloseSocket()
    {
        // Close the socket
        ws.Close();

        // Unregister the device
        using (UnityWebRequest wr = UnityWebRequest.Delete(baseURL + GingaURLTemplates.UnregisterSuffix(handle)))
        {
            yield return wr.SendWebRequest();
            if (wr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(wr.error);
            }
        }
    }
}
