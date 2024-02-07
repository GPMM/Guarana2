
public class Transition
{
    public string nodeid;
    public string ifaceid;
    public EventType evt;
    public EventTransition trans;


    public Transition(string nodeid, EventType evt, EventTransition trans)
    {
        this.nodeid = nodeid;
        this.evt = evt;
        this.trans = trans;
    }


    public Transition(string nodeid, string ifaceid, EventType evt, EventTransition trans)
    {
        this.nodeid = nodeid;
        this.ifaceid = ifaceid;
        this.evt = evt;
        this.trans = trans;
    }


    public override string ToString()
    {
        string ret = "(" + nodeid;
        if (ifaceid != null)
            ret += ", " + ifaceid;
        ret += ", " + evt.ToString() + ", " + trans.ToString() + ")";
        return ret;
    }


    public override bool Equals(object obj)
    {
        Transition t = obj as Transition;

        return !(t == null || t.nodeid != nodeid || t.ifaceid != ifaceid || t.evt != evt || t.trans != trans );
    }


    public override int GetHashCode()
    {
        int result = (nodeid != null ? nodeid.GetHashCode() : 0);
        result = (result * 397) ^ (ifaceid != null ? ifaceid.GetHashCode() : 0);
        result = (result * 397) ^ evt.GetHashCode();
        result = (result * 397) ^ trans.GetHashCode();

        return result;
    }
}
