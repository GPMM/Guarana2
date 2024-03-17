using System.Collections.Generic;

public class UnregisterDevice : WSStep
{
    
    void Start()
    {
        running = true;

        string handle = ((UnregisteringInput)input).handle;
        StartCoroutine(dtv.DeleteRemoteDevice(handle, CompleteUnregister));
    }

    
    void Update()
    {
        if (running) return;

        if (output != null)
        {
            Stop();
        }
    }


    private void CompleteUnregister(APIMessageBody result)
    {
        output = new WSStepIO();
        output.status = WSStepStatus.OK;

        running = false;
    }
}
