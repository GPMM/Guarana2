using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public enum WSStepStatus
{
    OK, ERROR
}

public class WSStepIO
{
    public WSStepStatus status;

    public override string ToString()
    {
        return "{\n\tstatus: " + status.ToString() + "\n}";
    }
}

public class DiscoveryOutput : WSStepIO
{
    public string hostIP;
    public string hostPort;

    public override string ToString()
    {
        return "{\n\tstatus: " + status.ToString() +
                "\n\thostIP: " + hostIP +
                "\n\thostPort: " + hostPort + "\n}";
    }
}

public class RegisterOutput : WSStepIO
{
    public string handle;
    public string url;

    public override string ToString()
    {
        return "{\n\tstatus: " + status.ToString() +
                "\n\thandle: " + handle +
                "\n\turl: " + url + "\n}";
    }
}

public class ConnectOutput : WSStepIO
{
    public string handle;
    public WebSocket ws;

    public override string ToString()
    {
        return "{\n\tstatus: " + status.ToString() +
                "\n\thandle: " + handle +
                "\n\tws: " + ws.ToString() + "\n}";
    }
}


public class IdentifyingInput : WSStepIO
{
    public bool reload;

    public override string ToString()
    {
        return "{\n\treload: " + reload.ToString() + "\n}";
    }
}