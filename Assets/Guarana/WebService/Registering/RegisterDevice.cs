using System.Collections.Generic;

public class RegisterDevice : WSStep
{
    private DTVRemoteDeviceBody msgBody;


    void Start()
    {
        msgBody = new DTVRemoteDeviceBody();
        msgBody.deviceClass = "Guarana";

        msgBody.supportedFormats = new List<string>();
        msgBody.supportedFormats.Add("application/x-ncl360");

        msgBody.recognizableEvents = new List<string>();
        msgBody.recognizableEvents.Add("view");

        running = true;
        StartCoroutine(dtv.PostRemoteDevice(msgBody, CompleteRegister));
    }

    
    void Update()
    {
        if (running) return;

        if (output != null)
        {
            Stop();
        }
    }


    private void CompleteRegister(DTVRemoteDeviceReturn result)
    {
        output = new ConnectInput();
        ((ConnectInput)output).handle = result.handle;
        ((ConnectInput)output).url = result.url;
        ((ConnectInput)output).resume = false;

        if (result.handle == null)
        {
            output.status = WSStepStatus.ERROR;
        }
        else
        {
            output.status = WSStepStatus.OK;
        }

        running = false;
    }
}
