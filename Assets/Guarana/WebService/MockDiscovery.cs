using UnityEngine;
using System;
using System.Net;
using System.Threading;

public class MockDiscovery : Discovery
{
    private HttpListener listener;
    private Thread listenerThread;
	private bool keepRunning;

    private int port = 65333;
	

    public MockDiscovery()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://+:" + port + "/");
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        listener.Start();

		listenerThread = new Thread(startListener);
        listenerThread.Start();
        Debug.Log("Http listener Started");
    }


    private void startListener()
    {
		keepRunning = true;
		while (keepRunning)
        {
            var result = listener.BeginGetContext(ListenerCallback, listener);
            result.AsyncWaitHandle.WaitOne();
        }
    }


	private void ListenerCallback(IAsyncResult result)
	{
		if (keepRunning)
		{
			var context = listener.EndGetContext(result);
            string ip = null, port = null;

            //Debug.Log(context.Request.RawUrl);

			if (context.Request.QueryString.AllKeys.Length > 0)
            {
				foreach (var key in context.Request.QueryString.AllKeys)
				{
                    if (key == "ip")
                    {
                        ip = context.Request.QueryString.GetValues(key)[0];
                    }
                    else if (key == "port")
                    {
                        port = context.Request.QueryString.GetValues(key)[0];
                    }
                }

			}

			context.Response.Close();

            if (ip != null && port != null)
            {
                location = new ServerLocation();
                location.ip = ip;
                location.port = port;
                keepRunning = false;
            }
		}
	}


	override
	public bool Running()
    {
		return keepRunning;
    }
}
