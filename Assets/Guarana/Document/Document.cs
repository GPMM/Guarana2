using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Document
{
    private Dictionary<string, Media> medias;
    private Scheduler scheduler;


    public Document()
    {
        medias = new Dictionary<string, Media>();
    }


    public void AddMedia(Media m)
    {
        m.SetDocument(this);
        medias.Add(m.GetId(), m);
    }


    public void SetScheduler(Scheduler scheduler)
    {
        this.scheduler = scheduler;
    }


    public void SetManager(GuaranaManager manager)
    {
        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            kvp.Value.SetManager(manager);
        }
    }


    public void EvalTick(float time)
    {
        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            kvp.Value.EvalTick(time);
        }
    }


    public void EvalAction(string nodeid, EventType eventType, EventTransition eventTransition)
    {
        medias[nodeid].EvalAction(eventType, eventTransition);
    }


    public void EvalEventTransition(string nodeid, EventType evt, EventTransition trans)
    {
        scheduler.AddEventTransition(nodeid, evt, trans);
    }


    public override string ToString()
    {
        string ret = "";
        int num = medias.Count;
        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            ret += kvp.Value.ToString();
            num--;
            if (num > 0) ret += "\n";
        }
        return ret;
    }
}
