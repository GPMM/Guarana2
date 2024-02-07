using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Image2DPlayer : Player
{

    void Start()
    {
        Initialize();
    }


    void Update()
    {
        CheckView();
    }


    public override void LoadContent(string src)
    {
        media.TriggerTransition(EventType.PREPARATION, EventTransition.START);
        StartCoroutine(manager.DownloadImage(src, this));
    }


    public void SetContent(Texture2D texture)
    {
        content.GetComponent<Renderer>().material.mainTexture = texture;
        media.TriggerTransition(EventType.PREPARATION, EventTransition.STOP);
    }


    public override void StartPresentation()
    {
        content.SetActive(true);
    }


    public override void StopPresentation()
    {
        content.SetActive(false);
    }


    public override void AbortPresentation()
    {
        content.SetActive(false);
    }


    public override void PausePresentation()
    {
        // do nothing
    }


    public override void ResumePresentation()
    {
        // do nothing
    }


    public override void ConfigureSound(string soundType, float volume)
    {
        // do nothing
    }
}
