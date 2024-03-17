using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GingaCCWSAPI
{
    public string BaseURL;
    public string AccessToken, RefreshToken, BindToken;
    public string PostMessageType = "application/json";

    public enum FileType { TEXT, IMAGE, AUDIO }


    public void SetBaseUrl(string hostIP, string hostPort)
    {
        BaseURL = "http://" + hostIP + ":" + hostPort;
    }


    public void SetBaseHeaders(UnityWebRequest wr)
    {
        wr.SetRequestHeader("Accept-Version", "1.0");
    }


    public void SetDTVHeaders(UnityWebRequest wr)
    {
        SetBaseHeaders(wr);
        wr.SetRequestHeader("accessToken", AccessToken.ToString());
    }


    public void SetCurrentServiceHeaders(UnityWebRequest wr)
    {
        SetDTVHeaders(wr);
        wr.SetRequestHeader("bind-token", BindToken.ToString());
    }


    public IEnumerator CreateRequestCoroutine<T>(UnityWebRequest wr, Action<T> callback)
    {
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
            throw new APIException(wr.responseCode, wr.error);

        var text = wr.downloadHandler.text;
        callback(JsonUtility.FromJson<T>(text));
    }


    public IEnumerator CreateRequestCoroutine<T>(UnityWebRequest wr, string id, Action<string, T> callback)
    {
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
            throw new APIException(wr.responseCode, wr.error);

        var text = wr.downloadHandler.text;
        callback(id, JsonUtility.FromJson<T>(text));
    }


    public IEnumerator CreateDownloadCoroutine<T>(UnityWebRequest wr, Action<T> callback)
    {
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
            throw new APIException(wr.responseCode, wr.error);

        if (typeof(T) == typeof(string))
        {
            callback((T)(object)wr.downloadHandler.text);
        }
        else if (typeof(T) == typeof(Texture2D))
        {
            callback((T)(object)DownloadHandlerTexture.GetContent(wr));
        }
        else if (typeof(T) == typeof(AudioClip))
        {
            callback((T)(object)DownloadHandlerAudioClip.GetContent(wr));
        }
        else
        {
            throw new APIException("Unsupported type");
        }
    }


    public IEnumerator CreateDownloadCoroutine<T>(UnityWebRequest wr, string id, Action<string, T> callback)
    {
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
            throw new APIException(wr.responseCode, wr.error);

        if (typeof(T) == typeof(string))
        {
            callback(id, (T)(object)wr.downloadHandler.text);
        }
        else if (typeof(T) == typeof(Texture2D))
        {
            callback(id, (T)(object)DownloadHandlerTexture.GetContent(wr));
        }
        else if (typeof(T) == typeof(AudioClip))
        {
            callback(id, (T)(object)DownloadHandlerAudioClip.GetContent(wr));
        }
        else
        {
            throw new APIException("Unsupported type");
        }
    }
}
