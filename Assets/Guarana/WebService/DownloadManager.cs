using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadManager : MonoBehaviour
{
    [SerializeField]
    private string baseLocation;
    [SerializeField]
    private string appId;

    
    public void SetBaseLocation(string baseLocation, string appId)
    {
        this.baseLocation = baseLocation;
        this.appId = appId;
    }


    private string SetupURL(string src)
    {
        if (src.StartsWith("http://") || src.StartsWith("https://"))
        {
            return src;
        }

        if (src.StartsWith("sbtvd-ts://"))
        {
            src = src.Replace("sbtvd-ts://", "");
            return baseLocation + GingaURLTemplates.StreamSuffix(appId, src);
        }

        if (src.StartsWith("/"))
        {
            src.Substring(1);
        }
        else if (src.StartsWith("./"))
        {
            src.Substring(2);
        }

        return baseLocation + GingaURLTemplates.AppFilesSuffix(appId, src);
    }


    public string SetupVideoURL(string src)
    {
        if (src.StartsWith("rtp://") || src.StartsWith("rtsp://") || src.StartsWith("http://") || src.StartsWith("https://"))
        {
            return src;
        }

        if (src.StartsWith("sbtvd-ts://"))
        {
            src = src.Replace("sbtvd-ts://", "");
            return baseLocation + GingaURLTemplates.StreamSuffix(appId, src);
        }

        if (src.StartsWith("/"))
        {
            src.Substring(1);
        }
        else if (src.StartsWith("./"))
        {
            src.Substring(2);
        }

        return baseLocation + GingaURLTemplates.StreamSuffix(appId, src);
    }


    public IEnumerator DownloadDocument(string src, GuaranaManager manager)
    {
        Debug.Log(SetupURL(src));
        using (UnityWebRequest wr = UnityWebRequest.Get(SetupURL(src)))
        {
            yield return wr.SendWebRequest();
            if (wr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(wr.error);
            }
            else
            {
                string xmldoc = wr.downloadHandler.text;
                manager.SetDocument(xmldoc);
            }
        }
    }


    public IEnumerator DownloadImage(string src, Player player)
    {
        Debug.Log(SetupURL(src));
        using (UnityWebRequest wr = UnityWebRequestTexture.GetTexture(SetupURL(src)))
        {
            yield return wr.SendWebRequest();
            if (wr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(wr.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(wr);
                ((Image2DPlayer)player).SetContent(texture);
            }
        }
    }


    public IEnumerator DownloadAudio(string src, Player player)
    {
        Debug.Log(SetupURL(src));
        using (UnityWebRequest wr = UnityWebRequestMultimedia.GetAudioClip(SetupURL(src), MimeTypes.GetAudioType(src)))
        {
            yield return wr.SendWebRequest();
            if (wr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(wr.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(wr);
                ((AudioPlayer) player).SetContent(clip);
            }
        }
    }


    public IEnumerator DownloadText(string src, Player player)
    {
        Debug.Log(SetupURL(src));
        using (UnityWebRequest wr = UnityWebRequest.Get(SetupURL(src)))
        {
            yield return wr.SendWebRequest();
            if (wr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(wr.error);
            }
            else
            {
                string text = wr.downloadHandler.text;
                ((TextPlayer) player).SetContent(text);
            }
        }
    }
}
