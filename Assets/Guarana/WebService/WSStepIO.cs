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

public class ConnectInput : WSStepIO
{
    public string nodeid;
    public string handle;
    public string url;
    public bool resume;

    public override string ToString()
    {
        return "{\n\tstatus: " + status.ToString() +
                "\n\tnodeid: " + nodeid +
                "\n\thandle: " + handle +
                "\n\turl: " + url +
                "\n\tresume: " + resume + "\n}";
    }
}

public class RunningInput : WSStepIO
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


public class UnregisteringInput : WSStepIO
{
    public string handle;

    public override string ToString()
    {
        return "{\n\tstatus: " + status.ToString() +
                "\n\thandle: " + handle + "\n}";
    }
}


public class PausedInput : WSStepIO
{
    public string nodeid;
    public string handle;
    public WebSocket ws;

    public override string ToString()
    {
        return "{\n\tstatus: " + status.ToString() +
                "\n\tnodeid: " + nodeid +
                "\n\thandle: " + handle +
                "\n\tws: " + ws.ToString() + "\n}";
    }
}