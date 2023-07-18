
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
		return GingaURLTemplates.BaseLocation(location.ip, location.port);
	}
}
