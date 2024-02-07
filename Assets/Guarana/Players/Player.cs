using UnityEngine;


public abstract class Player : MonoBehaviour
{
    [SerializeField]
    protected GameObject content;
    protected Media media;
    protected DownloadManager manager;

    protected bool inView;

    protected void Initialize()
    {
        content.SetActive(false);
        inView = false;
    }


    protected void CheckView()
    {
        if (content.activeInHierarchy)
        {
            Renderer rend = content.GetComponent<Renderer>();
            if (rend.isVisible && !inView)
            {
                media.TriggerTransition(EventType.VIEW, EventTransition.START);
                inView = true;
            }
            else if (!rend.isVisible && inView)
            {
                media.TriggerTransition(EventType.VIEW, EventTransition.STOP);
                inView = false;
            }
        }
    }


    public void SetMedia(Media media)
    {
        this.media = media;
    }


    public void SetDownloadManager(DownloadManager manager)
    {
        this.manager = manager;
    }


    public void SetPosition(float azimuthal, float polar, float radius)
    {
        content.transform.position = new Vector3(0, 0, radius);
        transform.Rotate(new Vector3(-polar, azimuthal, 0), Space.Self);
    }


    public void SetSize(float width, float height)
    {
        content.transform.localScale = new Vector3(width, height, 1);
    }


    public abstract void ConfigureSound(string soundType, float volume);

    public abstract void LoadContent(string src);

    public abstract void StartPresentation();

    public abstract void StopPresentation();

    public abstract void AbortPresentation();

    public abstract void PausePresentation();

    public abstract void ResumePresentation();
}
