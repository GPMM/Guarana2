using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GingaURLTemplates
{

    public static string BaseLocation(string ip, string port)
    {
        return "http://" + ip + ":" + port;
    }


    public static string RegisterSuffix()
    {
        return "/dtv/remote-device";
    }


    public static string UnregisterSuffix(string handle)
    {
        return "/dtv/remote-device/" + handle;
    }


    public static string UserListSuffix()
    {
        return "/dtv/user-api/user-list/";
    }


    public static string AppFilesSuffix(string appid, string src)
    {
        return "/dtv/current-service/apps/" + appid + "/files?path=" + src;
    }


    public static string StreamSuffix(string stream)
    {
        return "/dtv/current-service/stream/" + stream;
    }
}
