using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
    private class Action
    {
        public string nodeid;
        public EventType evt;
        public EventTransition trans;
        public float delay;
    }

    private class Transition
    {
        public string nodeid;
        public EventType evt;
        public EventTransition trans;
    }

    private Document doc;
    private List<Action> actions;
    private List<Action> delayedActions;
    private List<Transition> eventTransitions;
    private GuaranaManager manager;


    void Start()
    {
        actions = new List<Action>();
        delayedActions = new List<Action>();
        eventTransitions = new List<Transition>();
        manager = transform.parent.gameObject.GetComponent<GuaranaManager>();
    }

    
    void Update()
    {
        if (doc == null) return;

        doc.EvalTick(Time.deltaTime);

        if (delayedActions.Count > 0)
        {
            List<Action> temp = new List<Action>();
            foreach (Action act in delayedActions)
            {
                act.delay -= Time.deltaTime;
                if (act.delay <= 0)
                {
                    actions.Add(act);
                }
                else
                {
                    temp.Add(act);
                }
            }
            delayedActions = temp;
        }

        if (actions.Count > 0)
        {
            foreach (Action act in actions)
            {
                doc.EvalAction(act.nodeid, act.evt, act.trans);
            }
            actions.Clear();
        }

        if (eventTransitions.Count > 0)
        {
            foreach (Transition t in eventTransitions)
            {
                Debug.Log("Scheduler Trans: " + t.nodeid + " " + t.evt + " " + t.trans);
                manager.NotifyEventTransition(t.nodeid, t.evt, t.trans);
            }
            eventTransitions.Clear();
        }
    }


    public void SetDocument(Document doc)
    {
        this.doc = doc;
        doc.SetScheduler(this);
    }


    public void AddAction(string nodeid, EventType evt, EventTransition trans)
    {
        Debug.Log("Scheduler Action: " + nodeid + ", " + evt + ", " + trans);
        Action a = new Action();
        a.nodeid = nodeid;
        a.evt = evt;
        a.trans = trans;

        actions.Add(a);
    }


    public void AddDelayedAction(string nodeid, EventType evt, EventTransition trans, float delay)
    {
        Debug.Log("Scheduler DAction: " + nodeid + ", " + evt + ", " + trans + ", " + delay);
        Action a = new Action();
        a.nodeid = nodeid;
        a.evt = evt;
        a.trans = trans;
        a.delay = delay;

        delayedActions.Add(a);
    }


    public void AddEventTransition(string nodeid, EventType evt, EventTransition trans)
    {
        Transition t = new Transition();
        t.nodeid = nodeid;
        t.evt = evt;
        t.trans = trans;

        eventTransitions.Add(t);
    }
}
