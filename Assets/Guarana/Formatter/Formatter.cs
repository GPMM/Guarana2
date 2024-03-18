using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Formatter : MonoBehaviour
{
    public abstract GameObject GeneratePlayer(BaseMimeType mime, ScenePin pin);

    public abstract GameObject GenerateSkyPlayer(BaseMimeType mime);

    public abstract void DownloadContent<T>(string url, Action<T> callback);

    public abstract string SetupVideoURL(string src);
}
