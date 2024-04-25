#define TESTING

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScheduler : Formatter
{
    [SerializeField]
    private GameObject xrcamera;
    [SerializeField]
    private GameObject audioPlayerPrefab;
    [SerializeField]
    private GameObject image2DPlayerPrefab;
    [SerializeField]
    private GameObject textPlayerPrefab;
    [SerializeField]
    private GameObject video2DPlayerPrefab;
    [SerializeField]
    private GameObject video360PlayerPrefab;

    private GameObject scheduler;
    private GameObject scene;
    private List<Action> storedActionMsg;
    Document doc;

    float _delay;
    int _index;
    bool _run;


    void Awake()
    {
        scheduler = transform.Find("Scheduler").gameObject;
        scheduler.GetComponent<Scheduler>().notifyTransition = NotifyEventTransition;
        scheduler.GetComponent<Scheduler>().endDocument = RemoveDocument;

        scene = transform.Find("Scene").gameObject;

        storedActionMsg = new List<Action>();

        scheduler.SetActive(false);
    }


    void Start()
    {
        _delay = 1f;
        _index = 0;
        _run = true;

        //Debug.Log(FimaDoc());
        SetDocument(FimaDoc(), "fima");
        Debug.Log(doc.ToString());
    }


    void Update()
    {
        if (!_run) return;

        if (_delay > 0)
        {
            _delay -= Time.deltaTime;
            return;
        }

        scheduler.GetComponent<Scheduler>().AddAction(storedActionMsg[_index]);
        _index++;
        _delay = 1f;

        if (_index == storedActionMsg.Count)
        {
            _run = false;
        }        
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

        Debug.Log(t);
    }


    public void RemoveDocument()
    {
        for (int i = scene.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(scene.transform.GetChild(i).gameObject);
        }
        for (int i = xrcamera.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(xrcamera.transform.GetChild(i).gameObject);
        }
    }


    public override GameObject GeneratePlayer(BaseMimeType mime, ScenePin pin)
    {
        GameObject player;

        if (mime == BaseMimeType.audio)
        {
            player = Instantiate(audioPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (mime == BaseMimeType.image)
        {
            player = Instantiate(image2DPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (mime == BaseMimeType.text)
        {
            player = Instantiate(textPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (mime == BaseMimeType.video)
        {
            player = Instantiate(video2DPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            player = new GameObject();
        }

        player.transform.localPosition = new Vector3(0, 0, 0);
        player.transform.LookAt(Vector3.zero);

        player.GetComponent<Player>().SetFormatter(this);

        if (pin == ScenePin.ENVIRONMENT)
            player.transform.parent = scene.transform;
        else if (pin == ScenePin.CAMERA)
            player.transform.parent = xrcamera.transform;

        return player;
    }


    public override GameObject GenerateSkyPlayer(BaseMimeType mime)
    {
        GameObject player;

        player = Instantiate(video360PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        player.transform.localPosition = new Vector3(0, 0, 0);

        player.GetComponent<Player>().SetFormatter(this);
        player.transform.parent = scene.transform;

        return player;
    }


    public override void DownloadContent<T>(string url, Action<T> callback)
    {
        if (url.StartsWith("http"))
        {
            HTTP http = new HTTP();
            StartCoroutine(http.GetFile<T>(url, callback));
        }
        //else if (canRequestForService)
        //{
        //    StartCoroutine(dtv.CurrentService.GetAppsFiles<T>(sceneMetadata.appId, url, callback));
        //}
        else
        {
            Debug.Log("No bind toke available for Service Request");
        }
    }


    public override string SetupVideoURL(string src)
    {
        if (src.StartsWith("rtp://") || src.StartsWith("rtsp://") || src.StartsWith("http://") || src.StartsWith("https://"))
        {
            return src;
        }

        return null;
    }


    private string FimaDoc()
    {
        // add actions to be executed at each 1 sec
        //storedActionMsg.Add(new Action("main", EventType.PREPARATION, EventTransition.START));
        //storedActionMsg.Add(new Action("main", EventType.PRESENTATION, EventTransition.START));

        //storedActionMsg.Add(new Action("foto1", EventType.PREPARATION, EventTransition.START));
        //storedActionMsg.Add(new Action("foto1", EventType.PRESENTATION, EventTransition.START));

        //storedActionMsg.Add(new Action("palacio", EventType.PREPARATION, EventTransition.START));
        //storedActionMsg.Add(new Action("palacio", EventType.PRESENTATION, EventTransition.START));

        //storedActionMsg.Add(new Action("foto2", EventType.PRESENTATION, EventTransition.START));
        //storedActionMsg.Add(new Action("foto3", EventType.PRESENTATION, EventTransition.START));
        //storedActionMsg.Add(new Action("biografia", EventType.PRESENTATION, EventTransition.START));

        storedActionMsg.Add(new Action("fima", EventType.PREPARATION, EventTransition.START));
        storedActionMsg.Add(new Action("fima", EventType.PRESENTATION, EventTransition.START));

        //scheduler.GetComponent<Scheduler>().AddAction(new Action("fima", EventType.PRESENTATION, EventTransition.START));
        //scheduler.GetComponent<Scheduler>().AddAction(new Action("gualogo", EventType.PREPARATION, EventTransition.START));
        //scheduler.GetComponent<Scheduler>().AddDelayedAction(new Action("gualogo", EventType.PRESENTATION, EventTransition.START, 2f));
        //scheduler.GetComponent<Scheduler>().AddAction(new Action("gualogo", EventType.PRESENTATION, EventTransition.START));


        return "<ncl360 id=\"fima\">" +
            "<head>" +
                GenRegion("rtest", "0d", "0d", "0.5m", "0.2m", "0.5m", "environment") +
                GenRegion("rlog", "5d", "180d", "0.1m", "0.12m", "10m", "environment") +
                GenRegion("rbio", "10d", "160d", "3m", "7.5m", "6m", "environment") +
                GenRegion("rvid", "15d", "-105d", "3.2m", "2m", "8m", "environment") +
                GenRegion("rpic", "28d", "90d", "1.5m", "1m", "6m", "environment") +

                GenDescriptor("d360", "default.sky", "Equirectangular", "1", null) +
                GenDescriptor("dlog", "rlog", null, null, null) +
                GenDescriptor("dbio", "rbio", null, null, null) +
                GenDescriptor("dvid", "rvid", null, "0", null) +
                GenDescriptor("dpic", "rpic", null, null, null) +
                //GenDescriptor("d360", "rtest", null, "1", null) +
                //GenDescriptor("dlog", "rtest", null, null, null) +
                //GenDescriptor("dbio", "rtest", null, null, null) +
                //GenDescriptor("dvid", "rtest", null, "0", null) +
                //GenDescriptor("dpic", "rtest", null, null, null) +
            "</head>" +

            "<body>" +
                GenPort("main") +
                GenPort("gualogo") +

                GenMedia("main", "https://cursa.eic.cefet-rj.br/guarana-examples/fima/video360.mp4", "d360",
                        new List<(string, string, string)> {("scene1", "30s", "90s"), //30-90
                                                            ("scene2", "120s", "180s"),// 120-180
                                                            ("scene3", "210s", "300s")}) +//210-300

                GenMedia("gualogo", "https://cursa.eic.cefet-rj.br/guarana-examples/fima/logo.png", "dlog") +
                GenMedia("biografia", "https://cursa.eic.cefet-rj.br/guarana-examples/fima/biografia.png", "dbio") +
                GenMedia("palacio", "https://cursa.eic.cefet-rj.br/guarana-examples/fima/palacio2D.mp4", "dvid") +
                GenMedia("foto1", "https://cursa.eic.cefet-rj.br/guarana-examples/fima/dia.jpg", "dpic") +
                GenMedia("foto2", "https://cursa.eic.cefet-rj.br/guarana-examples/fima/estatua.jpg", "dpic") +
                GenMedia("foto3", "https://cursa.eic.cefet-rj.br/guarana-examples/fima/noite.jpg", "dpic") +

                GenLink("main", null, "onBegin", "palacio", "start", "0.5s") +

                GenLink("main", "scene1", "onBegin", "foto1", "start", null) +
                GenLink("main", "scene1", "onEnd",   "foto1", "stop", null) +

                GenLink("main", "scene2", "onBegin", "foto2", "start", null) +
                GenLink("main", "scene2", "onEnd",   "foto2", "stop", null) +

                GenLink("main", "scene3", "onBegin", "foto3", "start", null) +
                GenLink("main", "scene3", "onEnd",   "foto3", "stop", null) +

                GenLink("gualogo", null, "onBeginView", "biografia", "start", null) +
                GenLink("gualogo", null, "onEndView", "biografia", "stop", null) +
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
            ret += "projection=\"" + proj + "\" ";
        if (vol != null)
            ret += "volume=\"" + vol + "\" ";
        if (dur != null)
            ret += "dur=\"" + dur + "\" ";
        ret += "/>\n";
        return ret;
    }


    private string GenPort(string media)
    {
        return "<port component=\"" + media + "\"/>\n";
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
        ret += "component=\"" + cMedia + "\" ";
        if (cIface != null)
            ret += "interface=\"" + cIface + "\" ";
        ret += "role=\"" + cTrigger + "\" ";
        ret += "/>";

        ret += "<bind ";
        ret += "component=\"" + aMedia + "\" ";
        ret += "role=\"" + aTrigger + "\" ";
        if (aDelay != null)
            ret += "delay=\"" + aDelay + "\" ";
        ret += "/>";

        ret += "</link>\n";

        return ret;
    }
}
