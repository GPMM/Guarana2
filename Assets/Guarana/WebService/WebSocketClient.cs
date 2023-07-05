using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClient
{
    private WebSocket ws;
    private bool isRunning;


    public WebSocketClient(string url)
    {
        isRunning = false;

        ws = new WebSocket(url);

        ws.OnOpen += (sender, e) =>
        {
            isRunning = true;
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
        WebServiceUI.Log("Message:  " + data);
    }


    public void SendMessage(string data)
    {
        ws.Send(data);
    }


    public bool Running()
    {
        return isRunning;
    }
}
