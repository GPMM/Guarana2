using UnityEngine;
using TMPro;

public class WebServiceUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI console;
    private static string consoleText = "";

    private WebService ws;
    

    void Start()
    {
        ws = GetComponent<WebService>();
    }

    
    void Update()
    {
        console.text = consoleText;
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
