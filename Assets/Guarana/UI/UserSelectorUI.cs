using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSelectorUI : MonoBehaviour
{
    private UserSelector selector;

    
    void Start()
    {
        selector = GetComponent<UserSelector>();
    }

    
    void Update()
    {
        if (!selector.Running())
        {
            selector.SetUser("ana");
        }
    }
}
