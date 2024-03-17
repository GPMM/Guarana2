using System;

[Serializable]
public class APIException : Exception
{
    public APIException() : base() { }
    public APIException(string message) : base(message) { }
    public APIException(long code, string message) : base(code + " -- " + message) { }
    public APIException(string message, Exception inner) : base(message, inner) { }
}
