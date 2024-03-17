using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UserAPI
{
    private GingaCCWSAPI api;
    private string ApiURL;

    public UserAPI(GingaCCWSAPI api)
    {
        this.api = api;
        ApiURL = api.BaseURL + "/dtv/current-service/user-api";
    }


    public IEnumerator PostUserList(APIMessageBody msgBody, Action<DTVCSUserList> callback)
    {
        string url = ApiURL + "/user-list";
        string body = JsonUtility.ToJson(msgBody);
        using (UnityWebRequest wr = UnityWebRequest.Post(url, body, api.PostMessageType))
        {
            api.SetCurrentServiceHeaders(wr);
            yield return api.CreateRequestCoroutine<DTVCSUserList>(wr, callback);
        }
    }


    public IEnumerator GetUserAttribute(string userId, string attributeName, Action<string, DTVCSUserAttribute> callback)
    {
        string url = ApiURL + "/users/" + userId;
        if (attributeName != null)
            url += "?attribute=" + attributeName;

        using (UnityWebRequest wr = UnityWebRequest.Get(url))
        {
            api.SetCurrentServiceHeaders(wr);
            yield return api.CreateRequestCoroutine<DTVCSUserAttribute>(wr, userId, callback);
        }
    }


    public IEnumerator GetUserFile(string userId, string path, Action<string, Texture2D> callback)
    {
        string url = ApiURL + "/files?path=" + path;
        using (UnityWebRequest wr = UnityWebRequestTexture.GetTexture(url))
        {
            api.SetCurrentServiceHeaders(wr);
            yield return api.CreateDownloadCoroutine<Texture2D>(wr, userId, callback);
        }
    }
}
