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

        WebSocket ws = new WebSocket(((ConnectInput)input).url);
        ws.OnOpen += (sender, e) =>
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (((ConnectInput)input).resume)
            {
                NotifyTransition msg = new NotifyTransition();
                msg.node = ((ConnectInput)input).nodeid;
                msg.eventType = (EventType.PRESENTATION.ToString()).ToLower();
                msg.transition = (EventTransition.RESUME.ToString()).ToLower() + "s";

                ws.Send(JsonUtility.ToJson(msg));
            }

            output = new RunningInput();
            output.status = WSStepStatus.OK;
            ((RunningInput)output).handle = ((ConnectInput)input).handle;
            ((RunningInput)output).ws = ws;
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
