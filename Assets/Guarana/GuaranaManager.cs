using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuaranaManager : Formatter
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

    private GameObject webservice;
    private GameObject identifying;
    private GameObject scheduler;
    private GameObject scene;

    private UserData activeUser;
    private NodeMeta sceneMetadata;
    private List<Action> storedActions;

    private bool checkForUser, checkForScene;


    void Awake()
    {
        // Setup webservice before start running
        webservice = transform.Find("WebService").gameObject;
        webservice.GetComponent<WebService>().onRunning = WSOnRunning;
        webservice.GetComponent<WebService>().onMessage = WSOnMessage;
        webservice.SetActive(false);

        // Setup the user identification object
        identifying = transform.Find("Identifying").gameObject;
        identifying.SetActive(false);

        // Setup the scheduler
        scheduler = transform.Find("Scheduler").gameObject;
        scheduler.GetComponent<Scheduler>().notifyTransition = NotifyEventTransition;
        scheduler.GetComponent<Scheduler>().endDocument = RemoveDocument;
        scheduler.SetActive(false);

        // Setup remaining stuff
        scene = transform.Find("Scene").gameObject;
        storedActions = new List<Action>();
        checkForUser = false;
        checkForScene = false;
    }


    void Start()
    {
        webservice.SetActive(true);
    }


    void Update()
    {
        // Test if user identification is done
        if (checkForUser && !identifying.activeInHierarchy)
            OnUserIdentification();

        // Test if there is a scene to parse
        if (checkForScene)
            OnSceneReceive();

        // Test if there are actions to forward to the scheduler
        if (storedActions.Count > 0)
            ForwardActions();
    }


    public void WSOnRunning(bool isRunning)
    {
        if (isRunning && activeUser != null)
        {
            // webservice is at running stage, ask for user indentification
            DTV dtv = webservice.GetComponent<WebService>().GetDTV();
            identifying.GetComponent<Identifying>().Initialize(dtv, true);
            checkForUser = true;
        }

        //TODO: anything to do on pause?
    }


    public void WSOnMessage(MultidevMetadata metadata)
    {
        if (metadata is NodeMeta)
        {
            sceneMetadata = (NodeMeta) metadata;
            checkForScene = true;
        }
        else if (metadata is ActionMeta)
        {
            Action a;

            EventType type = (EventType)Enum.Parse(typeof(EventType), ((ActionMeta) metadata).eventType, true);
            EventTransition action = (EventTransition)Enum.Parse(typeof(EventTransition), ((ActionMeta)metadata).action, true);
            string node = ((ActionMeta)metadata).node;
            float delay = ((ActionMeta)metadata).delay;

            if (delay > 0)
            {
                a = new Action(node, type, action, ((ActionMeta)metadata).delay);
            }
            else
            {
                a = new Action(node, type, action);
            }

            storedActions.Add(a);
        }
    }


    private void OnUserIdentification()
    {
        checkForUser = false;
        activeUser = identifying.GetComponent<Identifying>().GetActiveUser();
    }

    
    private void OnSceneReceive()
    {
        checkForScene = false;

        DownloadContent<string>(sceneMetadata.nodeSrc, SetSceneXML);
    }

    
    private void ForwardActions()
    {
        List<Action> temp = new List<Action>();

        foreach (Action act in storedActions)
        {

            // If user identification is ongoing, only preparation messages can go through
            if (identifying.activeInHierarchy && act.evt != EventType.PREPARATION)
            {
                temp.Add(act);
                continue;
            }

            if (act.delay > 0)
            {
                scheduler.GetComponent<Scheduler>().AddDelayedAction(act);
            }
            else
            {
                scheduler.GetComponent<Scheduler>().AddAction(act);
            }

        }
        storedActions = temp;
    }

    
    public void SetSceneXML(string xmldoc)
    {
        XMLParser parser = new XMLParser();
        Document doc = parser.Parse(xmldoc, sceneMetadata.nodeId);
        doc.SetFormatter(this);

        scheduler.GetComponent<Scheduler>().SetDocument(doc);
        scheduler.SetActive(true);
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
        //Debug.Log("Received document termination");
    }

    
    public void NotifyEventTransition(string nodeid, EventType evt, EventTransition trans)
    {
        TransitionMeta metadata = new TransitionMeta();
        metadata.node = nodeid;
        metadata.eventType = Enum.GetName(typeof(EventType), evt).ToLower();
        metadata.transition = Enum.GetName(typeof(EventTransition), trans).ToLower() + "s";
        metadata.user = activeUser.id;

        webservice.GetComponent<WebService>().SendMessage(metadata);
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
        //else if (mime == BaseMimeType.model)
        //{
        //    //TODO
        //}
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

        //if (mime == BaseMimeType.image)
        //{
        //    //TODO
        //}
        //else if (mime == BaseMimeType.video)
        //{
        player = Instantiate(video360PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //}
        //else
        //{
        //    player = new GameObject();
        //}

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
            http.GetFile<T>(url, callback);
        }
        else
        {
            DTV dtv = webservice.GetComponent<WebService>().GetDTV();
            dtv.CurrentService.GetAppsFiles<T>(sceneMetadata.appId, url, callback);
        }
    }


    public override string SetupVideoURL(string src)
    {
        if (src.StartsWith("rtp://") || src.StartsWith("rtsp://") || src.StartsWith("http://") || src.StartsWith("https://"))
        {
            return src;
        }

        if (src.StartsWith("sbtvd-ts://"))
        {
            //TODO
        }

        if (src.StartsWith("/"))
        {
            src = src.Substring(1);
        }
        else if (src.StartsWith("./"))
        {
            src = src.Substring(2);
        }

        DTV dtv = webservice.GetComponent<WebService>().GetDTV();
        return dtv.CurrentService.GetAppsFilesURL(sceneMetadata.appId, src);
    }
}
