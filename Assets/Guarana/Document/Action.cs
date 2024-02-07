
public class Action
{
    public string nodeid;
    public EventType evt;
    public EventTransition trans;
    public float delay;


    public Action(string nodeid, EventType evt, EventTransition trans)
    {
        this.nodeid = nodeid;
        this.evt = evt;
        this.trans = trans;
    }


    public Action(string nodeid, EventType evt, EventTransition trans, float delay)
    {
        this.nodeid = nodeid;
        this.evt = evt;
        this.trans = trans;
        this.delay = delay;
    }


    public override string ToString()
    {
        string ret = "(" + nodeid + ", " + evt.ToString() + ", " + trans.ToString();
        if (delay > 0f)
            ret += ", " + delay.ToString();
        ret += ")";
        return ret;
    }
}
