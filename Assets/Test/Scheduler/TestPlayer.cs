//#define VERBOSE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : Player
{
    void Start()
    {
        //Initialize();
    }


    void Update()
    {
        //CheckView();
    }


    public new void SetPosition(float azimuthal, float polar, float radius)
    {
        // do nothing
    }


    public new void SetSize(float width, float height)
    {
        // do nothing
    }


    public override void LoadContent(string src)
    {
        media.TriggerTransition(EventType.PREPARATION, EventTransition.START);

#if VERBOSE
        string log = ">>>> Player.LoadContent(" + src + ")\n";
        log += ":::: media:" + media.GetId();
        Debug.Log(log);
#endif

        media.TriggerTransition(EventType.PREPARATION, EventTransition.STOP);
    }


    public override void StartPresentation()
    {
#if VERBOSE
        string log = ">>>> Player.StartPresentation()\n";
        log += ":::: media:" + media.GetId();
        Debug.Log(log);
#endif
    }


    public override void StopPresentation()
    {
#if VERBOSE
        string log = ">>>> Player.StopPresentation()\n";
        log += ":::: media:" + media.GetId();
        Debug.Log(log);
#endif
    }


    public override void AbortPresentation()
    {
#if VERBOSE
        string log = ">>>> Player.AbortPresentation()\n";
        log += ":::: media:" + media.GetId();
        Debug.Log(log);
#endif
    }


    public override void PausePresentation()
    {
#if VERBOSE
        string log = ">>>> Player.PausePresentation()\n";
        log += ":::: media:" + media.GetId();
        Debug.Log(log);
#endif
    }


    public override void ResumePresentation()
    {
#if VERBOSE
        string log = ">>>> Player.ResumePresentation()\n";
        log += ":::: media:" + media.GetId();
        Debug.Log(log);
#endif
    }


    public override void ConfigureSound(string soundType, float volume)
    {
        // do nothing
    }
}
