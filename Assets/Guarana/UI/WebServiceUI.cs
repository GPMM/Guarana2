using UnityEngine;
using TMPro;

public class WebServiceUI : MonoBehaviour
{
    [SerializeField]
    private Material emptySkyBox;
    [SerializeField]
    private TextMeshPro debugConsole;

    private static string consoleText = "";

    private WebService ws;
    

    void Start()
    {
        ws = GetComponent<WebService>();
        RenderSettings.skybox = emptySkyBox;
    }

    
    void Update()
    {
        debugConsole.text = consoleText;
    }


    public static void Log(string msg)
    {
        consoleText += msg;
    }


    public static void Clear()
    {
        consoleText = "";
    }
}
