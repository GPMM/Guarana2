
public enum EventType { PRESENTATION, PREPARATION, VIEW, SELECTION, FOCUS };
public enum EventState { OCCURRING, SLEEPING, PAUSED };
public enum EventTransition { START, STOP, ABORT, PAUSE, RESUME };

public class Event
{
    protected EventType mytype;
    protected EventState mystate;
    protected int occurrences;
    protected Media parent;


    public Event(EventType type)
    {
        this.mytype = type;
        this.mystate = EventState.SLEEPING;
    }


    public EventType Type()
    {
        return mytype;
    }


    public EventState State()
    {
        return mystate;
    }


    public int Occurrences()
    {
        return occurrences;
    }


    public bool Transition(EventTransition t)
    {
        switch (t)
        {
            case EventTransition.START:
                if (mystate != EventState.SLEEPING)
                    return false;

                mystate = EventState.OCCURRING;
                break;

            case EventTransition.STOP:
                if (mystate == EventState.SLEEPING)
                    return false;

                mystate = EventState.SLEEPING;
                occurrences++;
                break;

            case EventTransition.ABORT:
                if (mystate == EventState.SLEEPING)
                    return false;

                mystate = EventState.SLEEPING;
                break;

            case EventTransition.PAUSE:
                if (mystate != EventState.OCCURRING)
                    return false;

                mystate = EventState.PAUSED;
                break;

            case EventTransition.RESUME:
                if (mystate != EventState.PAUSED)
                    return false;

                mystate = EventState.OCCURRING;
                break;
        }

        return true;
    }


    public bool CheckTransition(EventTransition t)
    {
        switch (t)
        {
            case EventTransition.START:
                if (mystate != EventState.SLEEPING)
                    return false;

                break;

            case EventTransition.STOP:
                if (mystate == EventState.SLEEPING)
                    return false;

                break;

            case EventTransition.ABORT:
                if (mystate == EventState.SLEEPING)
                    return false;

                break;

            case EventTransition.PAUSE:
                if (mystate != EventState.OCCURRING)
                    return false;

                break;

            case EventTransition.RESUME:
                if (mystate != EventState.PAUSED)
                    return false;

                break;
        }

        return true;
    }
}