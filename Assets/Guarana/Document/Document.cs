using System.Collections.Generic;

public class Document
{
    private string id;
    private Event presentation, preparation;
    private Dictionary<string, Media> medias;
    private List<Action> ports;
    private Dictionary<Transition, List<Action>> links;
    private Scheduler scheduler;


    public Document(string id)
    {
        this.id = id;
        presentation = new Event(EventType.PRESENTATION);
        preparation = new Event(EventType.PREPARATION);

        medias = new Dictionary<string, Media>();
        ports = new List<Action>();
        links = new Dictionary<Transition, List<Action>>();
    }


    public void AddMedia(Media m)
    {
        m.SetDocument(this);
        medias.Add(m.GetId(), m);
    }


    public bool HasMedia(string nodeid)
    {
        return medias.ContainsKey(nodeid);
    }


    public void AddPort(Action action)
    {
        ports.Add(action);
    }


    public void AddLink(Transition condition, List<Action> actions)
    {
        if (!links.ContainsKey(condition))
        {
            links[condition] = new List<Action>();
        }

        foreach (Action a in actions)
        {
            links[condition].Add(a);
        }
    }


    public void SetScheduler(Scheduler scheduler)
    {
        this.scheduler = scheduler;
    }


    public void SetFormatter(Formatter formatter)
    {
        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            kvp.Value.SetFormatter(formatter);
        }
    }


    public bool Running()
    {
        return presentation.State() == EventState.OCCURRING;
    }


    public bool Ended()
    {
        return presentation.State() == EventState.SLEEPING && presentation.Occurrences() > 0;
    }


    public void EvalTick(float time)
    {
        if (!Running())
            return;

        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            kvp.Value.EvalTick(time);
        }
    }


    public void EvalAction(Action action)
    {
        // Test if is an action in the document
        if (action.nodeid == id && action.evt == EventType.PRESENTATION && presentation.Transition(action.trans))
        {
            switch (action.trans)
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
        else if (action.nodeid == id && action.evt == EventType.PREPARATION && preparation.Transition(action.trans))
        {
            StartPreparation();
        }
        else if (medias.ContainsKey(action.nodeid))
        {
            medias[action.nodeid].EvalAction(action);
        }
    }


    public void EvalEventTransition(Transition trans)
    {
        // Check links
        if (links.ContainsKey(trans))
        {
            foreach (Action a in links[trans])
            {
                scheduler.AddAction(a);
            }
        }

        if (trans.ifaceid == null)
            scheduler.AddEventTransition(trans);
    }


    public void UpdateState()
    {
        bool presenting = false;
        bool preparing = false;
        bool paused = false;

        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            presenting |= kvp.Value.IsPresenting();
            preparing |= kvp.Value.IsPreparing();
            paused |= kvp.Value.IsPaused();
        }

        // Update the PRESENTATION state, if it is the case
        EventTransition trans = EventTransition.STOP;
        if (presenting)
        {
            trans = EventTransition.START;
        }
        else if (paused)
        {
            trans = EventTransition.PAUSE;
        }

        if (presentation.Transition(trans))
        {
            EvalEventTransition(new Transition(id, EventType.PRESENTATION, trans));
        }

        // Update the PREPARATION state, if it is the case
        trans = EventTransition.STOP;
        if (preparing)
        {
            trans = EventTransition.START;
        }

        if (preparation.Transition(trans))
        {
            EvalEventTransition(new Transition(id, EventType.PREPARATION, trans));
        }
    }


    public void StartPreparation()
    {
        EvalEventTransition(new Transition(id, EventType.PREPARATION, EventTransition.START));

        foreach (KeyValuePair<string, Media> kvp in medias)
            kvp.Value.EvalAction(new Action(kvp.Value.GetId(), EventType.PREPARATION, EventTransition.START));
    }


    public void StartPresentation()
    {
        EvalEventTransition(new Transition(id, EventType.PRESENTATION, EventTransition.START));

        foreach (Action a in ports)
            scheduler.AddAction(a);
    }


    public void StopPresentation()
    {
        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            kvp.Value.EvalAction(new Action(kvp.Value.GetId(), EventType.PRESENTATION, EventTransition.STOP));
        }

        EvalEventTransition(new Transition(id, EventType.PRESENTATION, EventTransition.STOP));
    }


    public void AbortPresentation()
    {
        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            kvp.Value.EvalAction(new Action(kvp.Value.GetId(), EventType.PRESENTATION, EventTransition.ABORT));
        }

        EvalEventTransition(new Transition(id, EventType.PRESENTATION, EventTransition.ABORT));
    }


    public void PausePresentation()
    {
        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            kvp.Value.EvalAction(new Action(kvp.Value.GetId(), EventType.PRESENTATION, EventTransition.PAUSE));
        }

        EvalEventTransition(new Transition(id, EventType.PRESENTATION, EventTransition.PAUSE));
    }


    public void ResumePresentation()
    {
        EvalEventTransition(new Transition(id, EventType.PRESENTATION, EventTransition.RESUME));

        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            kvp.Value.EvalAction(new Action(kvp.Value.GetId(), EventType.PRESENTATION, EventTransition.RESUME));
        }
    }


    public override string ToString()
    {
        string ret = "MEDIAS \n";
        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            ret += ":: " + kvp.Value.ToString() + "\n";
        }

        // print ports
        ret += "PORTS \n";
        foreach (Action a in ports)
        {
            ret += ":: " + a.ToString() + "\n";
        }

        // print links
        foreach (KeyValuePair<Transition, List<Action>> kvp in links)
        {
            ret += ":: " + kvp.Key.ToString();
            ret += " -> ";
            foreach (Action a in kvp.Value)
                ret += a.ToString() + " ";
            ret += "\n";
        }
        return ret;
    }
}
