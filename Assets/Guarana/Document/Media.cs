using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ScenePin { ENVIRONMENT, CAMERA };
public enum ProjectionType { EQUIRECTANGULAR, EQUIANGULARCUBEMAP };

public class Media
{
    private Document doc;
    private string id, src, soundType;
    private float volume, dur, polar, azimuthal, radius, width, height;
    private int zIndex;
    private ScenePin pin;
    private ProjectionType proj;
    private bool inSky;

    private GuaranaManager manager;
    private GameObject player;
    private Event presentation, preparation, view;
    private float _dur;
    private bool _prepared, _autostart;


    public Media(string id)
    {
        this.id = id;

        //set default values for attributes
        soundType = "2D";
        volume = 0;
        width = 0;
        height = 0;
        zIndex = 0;
        pin = ScenePin.ENVIRONMENT;
        proj = ProjectionType.EQUIRECTANGULAR;
        inSky = false;
        dur = float.PositiveInfinity;

        presentation = new Event(EventType.PRESENTATION);
        preparation = new Event(EventType.PREPARATION);
        view = new Event(EventType.VIEW);

        _prepared = false;
        _autostart = false;
    }


    public string GetId() { return id; }

    public void SetDocument(Document doc) { this.doc = doc; }

    public void SetManager(GuaranaManager manager) { this.manager = manager; }

    public void SetSrc(string src) { this.src = src; }

    public void SetSoundType(string soundType) { this.soundType = soundType; }

    public void SetVolume(float volume) { this.volume = volume; }

    public void SetDur(float dur) { this.dur = dur; }

    public void SetPolar(float polar) { this.polar = polar; }

    public void SetAzimuthal(float azimuthal) { this.azimuthal = azimuthal; }

    public void SetRadius(float radius) { this.radius = radius; }

    public void SetWidth(float width) { this.width = width; }

    public void SetHeight(float height) { this.height = height; }

    public void SetZIndex(int zIndex) { this.zIndex = zIndex; }

    public void SetPin(ScenePin pin) { this.pin = pin; }

    public void SetProjection(ProjectionType proj) { this.proj = proj; }

    public void SetInSky() { inSky = true; }


    public void EvalTick(float time)
    {
        if (presentation.State() != EventState.OCCURRING)
            return;

        _dur -= time;
        if (_dur <= 0)
        {
            EvalAction(EventType.PRESENTATION, EventTransition.STOP);
        }
    }


    public void EvalAction(EventType evt, EventTransition trans)
    {
        if (evt == EventType.PREPARATION && trans == EventTransition.START && preparation.Transition(trans))
        {
            StartPreparation();
        }
        else if ( evt == EventType.PRESENTATION && presentation.Transition(trans) )
        {
            switch (trans)
            {
                case EventTransition.START:
                    StartPresentation();
                    break;
                case EventTransition.STOP:
                    StopPresentation();
                    break;
                case EventTransition.ABORT:
                    break;
                case EventTransition.PAUSE:
                    PausePresentation();
                    break;
                case EventTransition.RESUME:
                    ResumePresentation();
                    break;
            }
        }
    }


    public void TriggerTransition(EventType evt, EventTransition trans)
    {
        if (evt == EventType.VIEW && view.Transition(trans))
        {
            // nothing else to do, just send to document
            doc.EvalEventTransition(id, evt, trans);
        }
        else if (evt == EventType.PREPARATION && preparation.Transition(trans))
        {
            if (trans == EventTransition.STOP)
            {
                _prepared = true;

                if (_autostart)
                {
                    // start media after waiting for preparation
                    _autostart = false;
                    StartPresentation();
                }
            }

            // nothing else to do, just send to document
            doc.EvalEventTransition(id, evt, trans);
        }
        else if (evt == EventType.PRESENTATION && presentation.Transition(trans))
        {
            // check if natural end occurred
            if (trans == EventTransition.STOP)
            {
                StopPresentation();
            }

            // nothing else to do, just send to document
            doc.EvalEventTransition(id, evt, trans);
        }
    }


    public override string ToString()
    {
        string ret = "(" + id + ")";

        ret += " src:" + src;
        ret += " soundType:" + soundType;
        ret += " volume:" + volume;
        ret += " dur:" + dur;
        ret += " volume:" + volume;
        ret += " polar:" + polar;
        ret += " azimuthal:" + azimuthal;
        ret += " radius:" + radius;
        ret += " width:" + width;
        ret += " height:" + height;
        ret += " zIndex:" + zIndex;
        ret += " pin:" + pin;
        return ret;
    }


    private void StartPreparation()
    {
        _prepared = false;
        BaseMimeType mime = MimeTypes.GetBaseMimeType(src);

        if (inSky)
        {
            player = manager.GenerateSkyPlayer(mime);
        }
        else
        {
            player = manager.GeneratePlayer(mime, pin);
        }
        
        Player imp = player.GetComponent<Player>();
        imp.SetMedia(this);

        if (!inSky)
        {
            if (mime == BaseMimeType.text)
            {
                ((TextPlayer) imp).SetPosition(azimuthal, polar, radius);
                ((TextPlayer) imp).SetSize(width, height);
            }
            else
            {
                imp.SetPosition(azimuthal, polar, radius);
                imp.SetSize(width, height);
            }
        }
        else
        {
            ((Video360Player) imp).ConfigureProjection(proj);
        }

        if (mime == BaseMimeType.audio || mime == BaseMimeType.video)
        {
            imp.ConfigureSound(soundType, volume);
        }

        imp.LoadContent(src);
    }


    private void StartPresentation()
    {
        if (!_prepared)
        {
            // Media not prepared...
            if (preparation.State() == EventState.OCCURRING)
            {
                // ... wait for it end
                _autostart = true;
            }
            else
            {
                // ... otherwise prepare it first
                StartPreparation();
            }
        }
        else
        {
            _dur = dur;
            player.GetComponent<Player>().StartPresentation();
        }
    }


    private void StopPresentation()
    {
        player.GetComponent<Player>().StopPresentation();
    }


    private void PausePresentation()
    {
        player.GetComponent<Player>().PausePresentation();
    }


    private void ResumePresentation()
    {
        player.GetComponent<Player>().ResumePresentation();
    }
}
