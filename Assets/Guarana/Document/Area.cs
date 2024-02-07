
public class Area
{
    private string id;
    private float begin, end;

    private Media media;
    private bool _hasBegin, _hasEnd, _running;
    private bool _testBeg, _testEnd;
    private float _t;


    public Area(string id)
    {
        this.id = id;

        _hasBegin = false;
        _hasEnd = false;
    }


    public Area(string id, float begin, float end) : this(id)
    {
        SetBegin(begin);
        SetEnd(end);
    }


    public void SetBegin(float begin)
    {
        this.begin = begin;
        _hasBegin = true;
    }


    public void SetEnd(float end)
    {
        this.end = end;
        _hasEnd = true;
    }


    public void SetMedia(Media m)
    {
        media = m;
    }


    public bool Running()
    {
        return _running;
    }


    public void EvalTick(float time)
    {
        _t += time;

        if (_testBeg && (!_hasBegin || (_hasBegin && _t >= begin)))
        {
            _testBeg = false;
            _running = true;

            media.TriggerAreaTransition(id, EventType.PRESENTATION, EventTransition.START);
        }

        if (_testEnd && (!_hasEnd || (_hasEnd && _t >= end)))
        {
            _testEnd = false;
            _running = false;

            media.TriggerAreaTransition(id, EventType.PRESENTATION, EventTransition.STOP);
        }
    }


    public void StartPresentation()
    {
        _testBeg = true;
        _testEnd = true;

        _t = 0;
        _running = false;
    }


    public void StopPresentation()
    {
        if (!_running)
            return;

        _running = false;
        _testEnd = false;
        media.TriggerAreaTransition(id, EventType.PRESENTATION, EventTransition.STOP);
    }


    public void AbortPresentation()
    {
        if (!_running)
            return;

        _running = false;
        _testEnd = false;
        media.TriggerAreaTransition(id, EventType.PRESENTATION, EventTransition.ABORT);
    }


    public void PausePresentation()
    {
        if (!_running)
            return;

        media.TriggerAreaTransition(id, EventType.PRESENTATION, EventTransition.PAUSE);
    }


    public void ResumePresentation()
    {
        if (!_running)
            return;

        media.TriggerAreaTransition(id, EventType.PRESENTATION, EventTransition.RESUME);
    }


    public override string ToString()
    {
        string ret = "[" + id + ": ";
        if (_hasBegin)
            ret += begin;
        ret += "-";
        if (_hasEnd)
            ret += end;
        ret += "]";
        return ret;
    }
}
