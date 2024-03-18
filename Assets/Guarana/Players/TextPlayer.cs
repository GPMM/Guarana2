using UnityEngine;
using TMPro;

public class TextPlayer : Player
{

    void Start()
    {
        Initialize();
    }


    void Update()
    {
        CheckView();
    }


    public new void SetSize(float width, float height)
    {
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }


    public override void LoadContent(string src)
    {
        media.TriggerTransition(EventType.PREPARATION, EventTransition.START);
        formatter.DownloadContent<string>(src, SetContent);
    }


    public void SetContent(string text)
    {
        content.GetComponent<TextMeshPro>().text = text;
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
