using System.Collections.Generic;
using UnityEngine;
using Zeroconf;

public class MDNSDiscovery : WSStep
{
    private string serviceName = "_Ginga._tcp.local.";
    
    private float searchDelay = 2f;
    private float delta;

    
    void Start()
    {
        delta = searchDelay;

        SearchService();
    }

    
    void Update()
    {
        if (running) return;

        if (output != null)
        {
            Stop();
        }
        else
        {
            // Search again after a delay
            delta -= Time.deltaTime;
            if (delta <= 0)
            {
                delta = searchDelay;
                SearchService();
            }
        }
    }


    private async void SearchService()
    {
        running = true;
        IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync(serviceName);
        if (results.Count == 0)
        {
            Debug.Log("No service available");
            running = false;
            return;
        }

        // Get only the first service
        DiscoveryOutput myout = new DiscoveryOutput();
        myout.hostIP = results[0].IPAddress;
        foreach (KeyValuePair<string, IService> entry in results[0].Services)
        {
            myout.hostPort = entry.Value.Port.ToString();
        }

        myout.status = WSStepStatus.OK;
        output = myout;
        running = false;
    }
}
