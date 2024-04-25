using System.Collections.Generic;

public abstract class MultidevMetadata { }


[System.Serializable]
public class NodePropertyMeta
{
    public string name;
    public string value;
}


public class NodeMeta : MultidevMetadata
{
    public string nodeId;
    public string nodeSrc;
    public string appId;
    public string type;
    public List<NodePropertyMeta> properties;
}


public class ActionMeta : MultidevMetadata
{
    public string node;
    public string appId;
    public string eventType;
    public string action;
    public string value;
    public float delay;
}


public class TransitionMeta : MultidevMetadata
{
    public string node;
    public string appId;
    public string eventType;
    public string transition;
    public string value;
    public string user;
}