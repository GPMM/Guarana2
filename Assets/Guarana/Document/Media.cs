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
    
    private Formatter formatter;
    private GameObject player;
    private Event presentation, preparation, view;
    private List<Area> areas;
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

        areas = new List<Area>();

        _prepared = false;
        _autostart = false;
    }


    public string GetId() { return id; }

    public void SetDocument(Document doc) { this.doc = doc; }

    public void SetFormatter(Formatter formatter) { this.formatter = formatter; }

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


    public void AddArea(Area a)
    {
        a.SetMedia(this);
        areas.Add(a);
    }


    public bool IsOccurring()
    {
        return presentation.State() == EventState.OCCURRING || preparation.State() == EventState.OCCURRING;
    }


    public bool IsPaused()
    {
        return (presentation.State() == EventState.PAUSED || preparation.State() == EventState.PAUSED) &&
            (presentation.State() != EventState.OCCURRING && preparation.State() != EventState.OCCURRING);
    }


    public void EvalTick(float time)
    {
        if (presentation.State() != EventState.OCCURRING)
            return;

        _dur -= time;
        if (_dur <= 0)
        {
            EvalAction(new Action(id, EventType.PRESENTATION, EventTransition.STOP));
        }
        else
        {
            foreach (Area a in areas)
            {
                a.EvalTick(time);
            }
        }
    }


    public void EvalAction(Action a)
    {
        EventType evt = a.evt;
        EventTransition trans = a.trans;

        if (evt == EventType.PREPARATION && trans == EventTransition.START && preparation.Transition(trans))
        {
            StartPreparation();
        }
        else if (evt == EventType.PRESENTATION && trans == EventTransition.START && presentation.CheckTransition(trans) && !_prepared)
        {
            // Media not prepared...
            _autostart = true;
            if (preparation.State() == EventState.SLEEPING)
            {
                // ... prepare it first
                StartPreparation();
            }
        }
        else if ( evt == EventType.PRESENTATION && presentation.Transition(trans) )
        {
            switch (trans)
            {
                case EventTransition.START:
                    StartPresentation();
                    break;
                case EventTransition.STOP:
                    StopPresentation(true);
                    break;
                case EventTransition.ABORT:
                    AbortPresentation();
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
        bool _notify = false;
        bool _callstart = false;

        if (evt == EventType.VIEW && view.Transition(trans))
        {
            // nothing else to do, just send to document
            _notify = true;
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
                    _callstart = true;
                }
            }

            // nothing else to do, just send to document
            _notify = true;
        }
        else if (evt == EventType.PRESENTATION && presentation.Transition(trans))
        {
            // check if natural end occurred
            if (trans == EventTransition.STOP)
            {
                StopPresentation(false);
            }

            // nothing else to do, just send to document
            _notify = true;
        }

        if (_notify)
        {
            doc.EvalEventTransition(new Transition(id, evt, trans));
        }

        if (_callstart)
        {
            EvalAction(new Action(id, EventType.PRESENTATION, EventTransition.START));
        }
    }


    public void TriggerAreaTransition(string ifaceid, EventType evt, EventTransition trans)
    {
        doc.EvalEventTransition(new Transition(id, ifaceid, evt, trans));
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
        foreach (Area a in areas)
            ret += "\n" + a.ToString();
        return ret;
    }


    private void StartPreparation()
    {
        _prepared = false;
        BaseMimeType mime = MimeTypes.GetBaseMimeType(src);

        if (inSky)
        {
            player = formatter.GenerateSkyPlayer(mime);
        }
        else
        {
            player = formatter.GeneratePlayer(mime, pin);
        }
        
        Player imp = player.GetComponent<Player>();
        imp.SetMedia(this);

        if (!inSky)
        {
            if (mime == BaseMimeType.text)
            {
                ((TextPlayer)imp).SetPosition(azimuthal, polar, radius);
                ((TextPlayer)imp).SetSize(width, height);
            }
            else
            {
                imp.SetPosition(azimuthal, polar, radius);
                imp.SetSize(width, height);
            }
        }
        else
        {
            ((Video360Player)imp).ConfigureProjection(proj);
        }

        if (mime == BaseMimeType.audio || mime == BaseMimeType.video)
        {
            imp.ConfigureSound(soundType, volume);
        }

        imp.LoadContent(src);
    }


    private void StartPresentation()
    {
        _dur = dur;
        player.GetComponent<Player>().StartPresentation();
        doc.EvalEventTransition(new Transition(id, EventType.PRESENTATION, EventTransition.START));

        foreach (Area a in areas)
        {
            a.StartPresentation();
        }
    }


    private void StopPresentation(bool notify)
    {
        player.GetComponent<Player>().StopPresentation();
        if (notify)
            doc.EvalEventTransition(new Transition(id, EventType.PRESENTATION, EventTransition.STOP));

        foreach (Area a in areas)
        {
            a.StopPresentation();
        }
    }


    public void AbortPresentation()
    {
        player.GetComponent<Player>().AbortPresentation();
        doc.EvalEventTransition(new Transition(id, EventType.PRESENTATION, EventTransition.ABORT));

        foreach (Area a in areas)
        {
            a.AbortPresentation();
        }
    }


    private void PausePresentation()
    {
        player.GetComponent<Player>().PausePresentation();
        doc.EvalEventTransition(new Transition(id, EventType.PRESENTATION, EventTransition.PAUSE));

        foreach (Area a in areas)
        {
            a.PausePresentation();
        }
    }


    private void ResumePresentation()
    {
        player.GetComponent<Player>().ResumePresentation();
        doc.EvalEventTransition(new Transition(id, EventType.PRESENTATION, EventTransition.RESUME));

        foreach (Area a in areas)
        {
            a.ResumePresentation();
        }
    }
}
