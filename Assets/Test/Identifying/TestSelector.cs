using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestSelector : MonoBehaviour
{
    private GameObject selector;

    [SerializeField]
    private Texture2D[] thumbs;
    [SerializeField]
    private string[] names;

    private float delta = 2.0f;
    private bool run = true;


    void Start()
    {
        selector = transform.Find("Selector").gameObject;
        selector.SetActive(false);

        if (thumbs.Length != names.Length)
        {
            Debug.LogError("Thumbs and names have different sizes.");
            return;
        }

        List<UserData> tiles = new List<UserData>();
        for (int i = 0; i < thumbs.Length; i++)
        {
            UserData usr = new UserData();
            usr.name = names[i];
            usr.texture = thumbs[i];
            tiles.Add(usr);
        }

        selector.GetComponent<Selector>().SetInfo(tiles);
    }

    
    void Update()
    {
        if (run)
        {
            delta -= Time.deltaTime;
            if (delta <= 0)
            {
                selector.SetActive(true);
                run = false;
            }
        }

        if (!run && !selector.activeInHierarchy)
        {
            int user = selector.GetComponent<Selector>().GetCurrentIndex();
            Debug.Log("Selected user " + names[user]);

            //List<(Texture2D, string)> tiles = new List<(Texture2D, string)>();
            //tiles.Add((thumbs[user], names[user]));
            //selector.GetComponent<Selector>().SetInfo(tiles);
        }
    }
}
