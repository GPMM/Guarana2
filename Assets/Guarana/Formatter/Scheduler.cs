using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
    private Document doc;
    private List<Action> actions;
    private List<Action> delayedActions;
    private List<Transition> eventTransitions;
    private Formatter formatter;


    void Start()
    {
        actions = new List<Action>();
        delayedActions = new List<Action>();
        eventTransitions = new List<Transition>();
        formatter = transform.parent.gameObject.GetComponent<Formatter>();
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

        // execute actions from Ginga and those created from links
        while (actions.Count > 0)
        {
            List<Action> aux = new List<Action>(actions);
            actions.Clear();
            foreach (Action act in aux)
            {
                doc.EvalAction(act);
            }
        }

        doc.UpdateState();

        if (eventTransitions.Count > 0)
        {
            foreach (Transition t in eventTransitions)
            {
                formatter.NotifyEventTransition(t.nodeid, t.evt, t.trans);
            }
            eventTransitions.Clear();
        }
    }


    public void SetDocument(Document doc)
    {
        this.doc = doc;
        doc.SetScheduler(this);
    }


    public void AddAction(Action a)
    {
        //Debug.Log("Scheduler Action: " + a.nodeid + ", " + a.evt + ", " + a.trans);
        actions.Add(a);
    }


    public void AddDelayedAction(Action a)
    {
        //Debug.Log("Scheduler DAction: " + a.nodeid + ", " + a.evt + ", " + a.trans + ", " + a.delay);
        delayedActions.Add(a);
    }


    public void AddEventTransition(Transition t)
    {
        //Debug.Log("Scheduler Trans: " + t.nodeid + " " + t.evt + " " + t.trans);
        eventTransitions.Add(t);
    }
}
