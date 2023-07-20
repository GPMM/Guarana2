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
    private GuaranaManager manager;


    public WebSocketClient(string url, string handle, string baseURL, GuaranaManager manager)
    {
        this.handle = handle;
        this.baseURL = baseURL;
        this.manager = manager;
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
        //Debug.Log(data);
        if (data.Contains("appId"))
        {
            ReceiveScene response = JsonUtility.FromJson<ReceiveScene>(data);
            manager.ReceivedDocument(response);
        }
        else if (data.Contains("eventType"))
        {
            ReceiveAction response = JsonUtility.FromJson<ReceiveAction>(data);
            manager.ReceiveAction(response);
        }
        else
        {
            Debug.Log("Message not recognized");
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
