using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : Player
{
    private float _dur;


    void Start()
    {
        Initialize();
    }


    void Update()
    {
        if (content.GetComponent<AudioSource>().isPlaying)
        {
            _dur -= Time.deltaTime;
            if (_dur <= 0)
            {
                media.TriggerTransition(EventType.PRESENTATION, EventTransition.STOP);
            }
        }
    }


    public override void LoadContent(string src)
    {
        AudioSource audio = content.GetComponent<AudioSource>();
        audio.loop = false;

        media.TriggerTransition(EventType.PREPARATION, EventTransition.START);
        StartCoroutine(manager.DownloadAudio(src, this));
    }


    public void SetContent(AudioClip clip)
    {
        AudioSource audio = content.GetComponent<AudioSource>();
        audio.clip = clip;
        audio.clip.LoadAudioData();

        media.TriggerTransition(EventType.PREPARATION, EventTransition.STOP);
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
        AudioSource audio = content.GetComponent<AudioSource>();

        _dur = audio.clip.length;
        content.SetActive(true);
        audio.Play();
    }


    public override void StopPresentation()
    {
        content.SetActive(false);
        content.GetComponent<AudioSource>().Stop();
    }


    public override void AbortPresentation()
    {
        content.SetActive(false);
        content.GetComponent<AudioSource>().Stop();
    }


    public override void PausePresentation()
    {
        content.GetComponent<AudioSource>().Pause();
    }


    public override void ResumePresentation()
    {
        content.GetComponent<AudioSource>().Play();
    }
}
