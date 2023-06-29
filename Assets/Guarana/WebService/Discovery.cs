using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Discovery
{
	protected class ServerLocation
	{
		public string ip;
		public string port;
	}
	protected ServerLocation location;


	public abstract bool Running();


	public string GetLocation()
    {
		return "http://" + location.ip + ":" + location.port;
	}
}
