using UnityEngine;
using WebSocketSharp;

public class Paused : WSStep
{
    
    void Start()
    {
        running = true;
        WebSocket ws = ((PausedInput)input).ws;

        ws.Send(BuildTransitionMessage());
        ws.Close();
    }


    void Update()
    {
        if (running) return;

        if (output != null)
        {
            Stop();
        }
    }


    void OnApplicationPause(bool pauseStatus)
    {
        if (running && !pauseStatus)
        {
            // Exits the Paused state
            ResumeWebService();
        }
    }


    void OnApplicationFocus(bool hasFocus)
    {
        if (running && hasFocus)
        {
            // Exits the Paused state
            ResumeWebService();
        }
    }


    private void ResumeWebService()
    {
        output = new ConnectInput();
        ((ConnectInput)output).handle = ((PausedInput) input).handle;
        ((ConnectInput)output).url = ((PausedInput)input).ws.Url.OriginalString;
        ((ConnectInput)output).resume = true;
        ((ConnectInput)output).nodeid = ((PausedInput)input).nodeid;

        running = false;
    }


    private string BuildTransitionMessage()
    {
        NotifyTransition msg = new NotifyTransition();
        msg.node = ((PausedInput)input).nodeid;
        msg.eventType = (EventType.PRESENTATION.ToString()).ToLower();
        msg.transition = (EventTransition.PAUSE.ToString()).ToLower() + "s";

        return JsonUtility.ToJson(msg);
    }
}
