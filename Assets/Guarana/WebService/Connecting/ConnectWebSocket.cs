using UnityEngine;
using WebSocketSharp;

public class ConnectWebSocket : WSStep
{
    void Start()
    {
        Connect();
    }

    
    void Update()
    {
        if (running) return;

        if (output != null)
        {
            Stop();
        }
    }


    private void Connect()
    {
        running = true;

        WebSocket ws = new WebSocket(((RegisterOutput)input).url);
        ws.OnOpen += (sender, e) =>
        {
            if (!gameObject.activeInHierarchy)
                return;

            output = new ConnectOutput();
            output.status = WSStepStatus.OK;
            ((ConnectOutput)output).handle = ((RegisterOutput)input).handle;
            ((ConnectOutput)output).ws = ws;
            running = false;
        };

        ws.OnError += (sender, e) =>
        {
            if (!gameObject.activeInHierarchy)
                return;

            Debug.Log(e.Message);
            output = new WSStepIO();
            output.status = WSStepStatus.ERROR;
            running = false;
        };

        ws.Connect();
    }
}
