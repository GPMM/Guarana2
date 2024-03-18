using UnityEngine;
using TMPro;

public class WebServiceUI : MonoBehaviour
{
    [SerializeField]
    private Material emptySkyBox;
    

    void Start()
    {
        RenderSettings.skybox = emptySkyBox;
    }
}
