using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class MockDiscovery : Discovery
{
    private HttpListener listener;
    private Thread listenerThread;
	private bool keepRunning;

    private int port = 65333;


    public MockDiscovery()
    {
        WebServiceUI.Log("Mock discovery ");
        listener = new HttpListener();
        port = FreeTcpPort();
        listener.Prefixes.Add("http://+:" + port + "/");
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

        listener.Start();
        WebServiceUI.Log(". ");

        listenerThread = new Thread(startListener);
        listenerThread.Start();
        WebServiceUI.Log(". ");
    }


    static int FreeTcpPort()
    {
        WebServiceUI.Log(". ");
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        WebServiceUI.Log("on port " + port + "\n");
        return port;
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
        WebServiceUI.Log("Got a connection\n");
        if (keepRunning)
		{
			var context = listener.EndGetContext(result);
            string ip = null, port = null;

            //WebServiceUI.Log(context.Request.RawUrl);

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
