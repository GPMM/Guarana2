using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HTTP
{

    public IEnumerator GetFile<T>(string url, Action<T> callback)
    {
        UnityWebRequest wr;
        if (typeof(T) == typeof(string))
        {
            wr = UnityWebRequest.Get(url);
        }
        else if (typeof(T) == typeof(Texture2D))
        {
            wr = UnityWebRequestTexture.GetTexture(url);
        }
        else if (typeof(T) == typeof(AudioClip))
        {
            string ext = Path.GetExtension(url);
            AudioType atype;

            if (ext == ".mp3") atype = AudioType.MPEG;
            else if (ext == ".ogg") atype = AudioType.OGGVORBIS;
            else atype = AudioType.UNKNOWN;

            wr = UnityWebRequestMultimedia.GetAudioClip(url, atype);
        }
        else
        {
            throw new APIException("Unsupported type");
        }

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
}
