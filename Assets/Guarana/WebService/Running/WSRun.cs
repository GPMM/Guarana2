using UnityEngine;
using WebSocketSharp;

public class WSRun : WSStep
{
    
    void Start()
    {
        running = true;
        WebSocket ws = ((RunningInput)input).ws;
        ws.OnClose += CloseSocket;
        ws.OnError += SocketError;
        ws.OnMessage += ReceiveMessage;
    }


    void Update()
    {
        if (running) return;

        if (output != null)
        {
            Stop();
        }
    }


    public void ReceiveMessage(object sender, MessageEventArgs e)
    {
        string data = e.Data;
        if (data.Contains("nodeId"))
        {
            NodeMeta metadata = JsonUtility.FromJson<NodeMeta>(data);
            ((RunningInput)input).messageHandler(metadata);
        }
        else if (data.Contains("eventType"))
        {
            ActionMeta metadata = JsonUtility.FromJson<ActionMeta>(data);
            ((RunningInput)input).messageHandler(metadata);
        }
        else
        {
            Debug.Log("Message not recognized");
        }
        //Debug.Log(data);
    }


    public void SendMessage(MultidevMetadata msg)
    {
        string data = JsonUtility.ToJson(msg);
        ((RunningInput)input).ws.Send(data);
    }


    public void SocketError(object sender, ErrorEventArgs e)
    {
        if (!gameObject.activeInHierarchy)
            return;

        Debug.Log(e.Message);
        output = new WSStepIO();
        output.status = WSStepStatus.ERROR;

        running = false;
    }


    public void CloseSocket(object sender, CloseEventArgs e)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (output == null)
        {
            // Got no heads up
            Debug.Log(e.Reason);
            output = new WSStepIO();
            output.status = WSStepStatus.ERROR;
        }

        running = false;
    }


    public void Pause()
    {
        output = new PausedInput();
        output.status = WSStepStatus.OK;
        ((PausedInput)output).ws = ((RunningInput)input).ws;
        ((PausedInput)output).handle = ((RunningInput)input).handle;

        running = false;
    }
}
