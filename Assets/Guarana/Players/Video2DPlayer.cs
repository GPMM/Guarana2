using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Video2DPlayer : Player
{
    private bool _checkPrepare = false;

    void Start()
    {
        Initialize();
    }


    void Update()
    {
        CheckView();

        if (_checkPrepare)
        {
            Prepared(content.GetComponent<VideoPlayer>());
        }
    }


    public override void LoadContent(string src)
    {
        media.TriggerTransition(EventType.PREPARATION, EventTransition.START);

        RenderTexture rt = new RenderTexture(256, 256, 16);
        rt.Create();

        content.GetComponent<Renderer>().material.mainTexture = rt;

        VideoPlayer video = content.GetComponent<VideoPlayer>();
        video.targetTexture = rt;
        video.url = formatter.SetupVideoURL(src);
        video.prepareCompleted += Prepared;
        video.loopPointReached += NaturalEnd;
        video.Prepare();
        _checkPrepare = true;
    }


    public void Prepared(VideoPlayer player)
    {
        //if (player.isPrepared) //Bug: Prepare never finishes
        //{
            _checkPrepare = false;
            media.TriggerTransition(EventType.PREPARATION, EventTransition.STOP);
        //}
    }


    public void NaturalEnd(VideoPlayer player)
    {
        media.TriggerTransition(EventType.PRESENTATION, EventTransition.STOP);
    }


    public override void ConfigureSound(string soundType, float volume)
    {
        AudioSource audio = content.GetComponent<AudioSource>();
        if (soundType == "2D")
            audio.spatialBlend = 0;
        else
            audio.spatialBlend = 1;

        audio.volume = volume;
    }


    public override void StartPresentation()
    {
        content.SetActive(true);
        content.GetComponent<VideoPlayer>().Play();
    }


    public override void StopPresentation()
    {
        content.SetActive(false);
        content.GetComponent<VideoPlayer>().Stop();
    }


    public override void AbortPresentation()
    {
        content.SetActive(false);
        content.GetComponent<VideoPlayer>().Stop();
    }


    public override void PausePresentation()
    {
        content.GetComponent<VideoPlayer>().Pause();
    }


    public override void ResumePresentation()
    {
        content.GetComponent<VideoPlayer>().Play();
    }
}
