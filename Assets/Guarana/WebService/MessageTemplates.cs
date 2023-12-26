using System.Collections.Generic;

public class RegisterMessage
{
    public string deviceClass;
    public List<string> supportedFormats;
    public List<string> recognizableEvents;

    public RegisterMessage()
    {
        deviceClass = "Guarana";

        supportedFormats = new List<string>();
        supportedFormats.Add("application/x-ncl360");

        recognizableEvents = new List<string>();
        recognizableEvents.Add("selection");
        recognizableEvents.Add("view");
    }
}


public class RegisterResponse
{
    public string handle;
    public string url;
}


[System.Serializable]
public class UserInfo
{
    public string id;
    public string name;
    public string icon;
}

public class UserListResponse
{
    public List<UserInfo> users;
}


public abstract class APIMessage { }


public class NotifyUser : APIMessage
{
    public string user;
}


public class ReceiveScene : APIMessage
{
    public string nodeSrc;
    public string appId;
    public string type;
    public List<string> notifyEvents;
}


public class ReceiveAction : APIMessage
{
    public string node;
    public string eventType;
    public string action;
    public int delay;
}


public class NotifyTransition : APIMessage
{
    public string node;
    public string eventType;
    public string transition;
    //public int value;
}
