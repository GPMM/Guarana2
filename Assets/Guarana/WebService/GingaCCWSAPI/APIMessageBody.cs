using System.Collections.Generic;

public class APIMessageBody { }


public class DTVErrorMessage : APIMessageBody
{
    public long ErrorCode;

    public DTVErrorMessage(long code) { ErrorCode = code; }
}


public class DTVAuthorizeReturn : APIMessageBody
{
    public string challenge;

    public override string ToString()
    {
        return "{\n\tchallenge:" + challenge + "\n}";
    }
}


public class DTVTokenReturn : APIMessageBody
{
    public string accessToken;
    public string tokenType;
    public float expiresIn;
    public string refreshToken;
    public string serverCert;

    public override string ToString()
    {
        return "{\n\taccessToken: " + accessToken +
                "\n\ttokenType: " + tokenType +
                "\n\texpiresIn: " + expiresIn +
                "\n\trefreshToken: " + refreshToken +
                "\n\tserverCert: " + serverCert + "\n}";
    }
}


public class DTVRemoteDeviceBody : APIMessageBody
{
    public string deviceClass;
    public List<string> supportedFormats;
    public List<string> recognizableEvents;
}


public class DTVRemoteDeviceReturn : APIMessageBody
{
    public string handle;
    public string url;

    public override string ToString()
    {
        return "{\n\thandle: " + handle +
                "\n\turl: " + url + "\n}";
    }
}


public class DTVCurrentServiceReturn : APIMessageBody
{
    public string serviceContextId;
    public string serviceName;
    public string transportStreamId;
    public string originalNetworkId;
    public string serviceId;

    public override string ToString()
    {
        return "{\n\tserviceContextId: " + serviceContextId +
                "\n\tserviceName: " + serviceName +
                "\n\ttransportStreamId: " + transportStreamId +
                "\n\toriginalNetworkId: " + originalNetworkId +
                "\n\tserviceId: " + serviceId + "\n}";
    }
}


[System.Serializable]
public class User
{
    public string id;

    public override string ToString()
    {
        return id;
    }
}


public class DTVCSUserList : APIMessageBody
{
    public List<User> users;

    public override string ToString()
    {
        string r = "[";
        foreach (User user in users)
        {
            r += user + ", ";
        }
        return r + "]";
    }
}


public class DTVCSUserAttribute : APIMessageBody
{
    public string id;
    public string name;
    public int age;
    public bool isGroup;
    public string gender;
    public string avatar;
    public string language;
}