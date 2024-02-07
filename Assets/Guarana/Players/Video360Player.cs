using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Video360Player : Player
{
    [SerializeField]
    private Material emptySkyBox;
    [SerializeField]
    private Material equiRectMaterial;
    [SerializeField]
    private RenderTexture equiRectTexture;
    [SerializeField]
    private Material equiAngCubeMaterial;
    [SerializeField]
    private RenderTexture equiAngCubeTexture;

    private Material _material;


    void Start()
    {
        Initialize();
    }


    void Update()
    {
        //CheckView();
    }


    public void ConfigureProjection(ProjectionType proj)
    {
        VideoPlayer video = content.GetComponent<VideoPlayer>();

        // change the video player texture and skybox material
        if (proj == ProjectionType.EQUIANGULARCUBEMAP)
        {
            video.targetTexture = equiAngCubeTexture;
            _material = equiAngCubeMaterial;
        }
        else
        {
            // Default is equirectangular
            video.targetTexture = equiRectTexture;
            _material = equiRectMaterial;
        }
    }


    public override void LoadContent(string src)
    {
        media.TriggerTransition(EventType.PREPARATION, EventTransition.START);

        VideoPlayer video = content.GetComponent<VideoPlayer>();
        video.url = manager.SetupVideoURL(src);
        video.prepareCompleted += Prepared;
        video.loopPointReached += NaturalEnd;
        video.Prepare();
        Prepared(video);
    }


    public void Prepared(VideoPlayer player)
    {
        media.TriggerTransition(EventType.PREPARATION, EventTransition.STOP);
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
        RenderSettings.skybox = _material;
    }


    public override void StopPresentation()
    {
        content.SetActive(false);
        content.GetComponent<VideoPlayer>().Stop();
        RenderSettings.skybox = emptySkyBox;
    }


    public override void AbortPresentation()
    {
        content.SetActive(false);
        content.GetComponent<VideoPlayer>().Stop();
        RenderSettings.skybox = emptySkyBox;
    }


    public override void PausePresentation()
    {
        content.GetComponent<VideoPlayer>().Pause();
    }


    public override void ResumePresentation()
    {
        content.GetComponent<VideoPlayer>().Play();
    }


    public new void SetPosition(float azimuthal, float polar, float radius)
    {
        // do nothing
    }


    public new void SetSize(float width, float height)
    {
        // do nothing
    }
}
