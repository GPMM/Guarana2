#define TESTING

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScheduler : Formatter
{
    private GameObject scheduler;
    private List<Action> storedActionMsg;
    Document doc;
    float _initdelay;
    bool _run;


    void Awake()
    {
        scheduler = transform.Find("Scheduler").gameObject;
        scheduler.GetComponent<Scheduler>().notifyTransition = NotifyEventTransition;
        scheduler.GetComponent<Scheduler>().endDocument = RemoveDocument;

        storedActionMsg = new List<Action>();

        scheduler.SetActive(false);
    }


    void Start()
    {
        _initdelay = 1.5f;
        _run = true;

        //Debug.Log(FimaDoc());
        SetDocument(FimaDoc(), "fima");
        Debug.Log(doc.ToString());
    }


    void Update()
    {
        if (!_run) return;

        if (_initdelay > 0)
        {
            _initdelay -= Time.deltaTime;
            return;
        }

        // add actions to scheduler
        scheduler.GetComponent<Scheduler>().AddAction(new Action("fima", EventType.PRESENTATION, EventTransition.START));

        //scheduler.GetComponent<Scheduler>().AddAction(new Action("gualogo", EventType.PREPARATION, EventTransition.START));
        //scheduler.GetComponent<Scheduler>().AddDelayedAction(new Action("gualogo", EventType.PRESENTATION, EventTransition.START, 2f));

        //scheduler.GetComponent<Scheduler>().AddAction(new Action("gualogo", EventType.PRESENTATION, EventTransition.START));

        _run = false;
    }


    public void SetDocument(string xmldoc, string id)
    {
        XMLParser parser = new XMLParser();
        doc = parser.Parse(xmldoc, id);
        doc.SetFormatter(this);

        scheduler.GetComponent<Scheduler>().SetDocument(doc);
        scheduler.SetActive(true);
    }


    public void NotifyEventTransition(string nodeid, EventType evt, EventTransition trans)
    {
        string t = "Notify: (";

        t += nodeid + ", ";
        t += Enum.GetName(typeof(EventType), evt).ToLower() + ", ";
        t += Enum.GetName(typeof(EventTransition), trans).ToLower();
        t += ")";

        //Debug.Log(t);
    }


    public void RemoveDocument()
    {

    }


    public override GameObject GeneratePlayer(BaseMimeType mime, ScenePin pin)
    {
        GameObject player = new GameObject();
        player.AddComponent<TestPlayer>();

        return player;
    }


    public override GameObject GenerateSkyPlayer(BaseMimeType mime)
    {
        GameObject player = new GameObject();
        player.AddComponent<TestPlayer>();

        return player;
    }


    public override void DownloadContent<T>(string url, Action<T> callback)
    {
        
    }


    public override string SetupVideoURL(string src)
    {
        return src;
    }


    private string FimaDoc()
    {
        return "<ncl360 id=\"fima\">" +
            "<head>" +
                GenRegion("rlog", "5d", "180d", "0.1m", "0.12m", "10m", "environment") +
                GenRegion("rbio", "10d", "160d", "3m", "7.5m", "6m", "environment") +
                GenRegion("rvid", "15d", "-105d", "3.2m", "2m", "8m", "environment") +
                GenRegion("rpic", "28d", "90d", "1.5m", "1m", "6m", "environment") +

                //GenDescriptor("d360", "default.sky", "Equirectangular", "1", null) +
                GenDescriptor("d360", "rlog", null, "1", null) +
                GenDescriptor("dlog", "rlog", null, null, null) +
                GenDescriptor("dbio", "rbio", null, null, null) +
                GenDescriptor("dvid", "rvid", null, "0", null) +
                GenDescriptor("dpic", "rpic", null, null, null) +
            "</head>" +

            "<body>" +
                GenPort("main") +
                GenPort("gualogo") +

                GenMedia("main", "media/video360.mp4", "d360",
                        new List<(string, string, string)> {("scene1", "1s", "3s"), //30-90
                                                            ("scene2", "5s", "7s"),// 120-180
                                                            ("scene3", "9s", "11s")}) +//210-300

                GenMedia("gualogo", "media/logo.png", "dlog") +
                GenMedia("biografia", "media/biografia.png", "dbio") +
                GenMedia("palacio", "media/palacio2D.mp4", "dvid") +
                GenMedia("foto1", "media/dia.jpg", "dpic") +
                GenMedia("foto2", "media/estatua.jpg", "dpic") +
                GenMedia("foto3", "media/noite.jpg", "dpic") +

                GenLink("main", null, "onBegin", "palacio", "start", "0.5s") +

                GenLink("main", "scene1", "onBegin", "foto1", "start", null) +
                GenLink("main", "scene1", "onEnd",   "foto1", "stop", null) +

                GenLink("main", "scene2", "onBegin", "foto2", "start", null) +
                GenLink("main", "scene2", "onEnd",   "foto2", "stop", null) +

                GenLink("main", "scene3", "onBegin", "foto3", "start", null) +
                GenLink("main", "scene3", "onEnd",   "foto3", "stop", null) +

                GenLink("gualogo", null, "onEnterView", "biografia", "start", null) +
                GenLink("gualogo", null, "onExitView", "biografia", "stop", null) +
            "</body>" +
        "</ncl360>";
    }


    private string GenRegion(string id, string polar, string azimuthal, string width, string height, string radius, string pin)
    {
        string ret = "<region ";
        ret += "id=\"" + id + "\" ";
        ret += "polar=\"" + polar + "\" ";
        ret += "azimuthal=\"" + azimuthal + "\" ";

        if (width != null)
            ret += "width=\"" + width + "\" ";
        if (height != null)
            ret += "height=\"" + height + "\" ";
        if (radius != null)
            ret += "radius=\"" + radius + "\" ";
        if (pin != null)
            ret += "pin=\"" + pin + "\" ";
        ret += "/>\n";
        return ret;
    }


    private string GenDescriptor(string id, string reg, string proj, string vol, string dur)
    {
        string ret = "<descriptor ";
        ret += "id=\"" + id + "\" ";
        ret += "region=\"" + reg + "\" ";
        if (proj != null)
            ret += "project =\"" + proj + "\" ";
        if (vol != null)
            ret += "volume=\"" + vol + "\" ";
        if (dur != null)
            ret += "dur=\"" + dur + "\" ";
        ret += "/>\n";
        return ret;
    }


    private string GenPort(string media)
    {
        return "<port media=\"" + media + "\"/>\n";
    }


    private string GenMedia(string id, string src, string desc, List<(string, string, string)> areas)
    {
        string ret = "<media ";
        ret += "id=\"" + id + "\" ";
        ret += "src=\"" + src + "\" ";
        ret += "descriptor=\"" + desc + "\" ";
        if (areas.Count > 0)
        {
            ret += ">\n";
            foreach ((string, string, string) a in areas)
            ret += GenArea(a.Item1, a.Item2, a.Item3);
            ret += "</media>\n";
        }
        else
        {
            ret += "/>\n";
        }
        return ret;
    }


    private string GenArea(string id, string begin, string end)
    {
        string ret = "<area ";
        ret += "id=\"" + id + "\" ";
        if (begin != null)
            ret += "begin=\"" + begin + "\" ";
        if (end != null)
            ret += "end=\"" + end + "\" ";
        ret += "/>\n";
        return ret;
    }


    private string GenMedia(string id, string src, string desc)
    {
        return "<media id=\"" + id + "\" src=\"" + src + "\" descriptor=\"" + desc + "\"/>\n";
    }


    private string GenLink(string cMedia, string cIface, string cTrigger, string aMedia, string aTrigger, string aDelay)
    {
        string ret = "<link>";

        ret += "<bind ";
        ret += "media=\"" + cMedia + "\" ";
        if (cIface != null)
            ret += "interface=\"" + cIface + "\" ";
        ret += "trigger=\"" + cTrigger + "\" ";
        ret += "/>";

        ret += "<bind ";
        ret += "media=\"" + aMedia + "\" ";
        ret += "trigger=\"" + aTrigger + "\" ";
        if (aDelay != null)
            ret += "delay=\"" + aDelay + "\" ";
        ret += "/>";

        ret += "</link>\n";

        return ret;
    }
}
