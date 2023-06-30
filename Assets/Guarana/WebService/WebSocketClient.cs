using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClient
{
    private WebSocket ws;


    public WebSocketClient(string url)
    {
        ws = new WebSocket(url);
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            ReceiveMessage(e.Data);
        };
    }


    private void ReceiveMessage(string data)
    {
        Debug.Log("Message:  " + data);
    }


    public void SendMessage(string data)
    {
        ws.Send(data);
    }
}
