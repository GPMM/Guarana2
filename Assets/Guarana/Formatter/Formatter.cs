using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Formatter : MonoBehaviour
{
    public abstract void NotifyEventTransition(string nodeid, EventType evt, EventTransition trans);

    public abstract GameObject GeneratePlayer(BaseMimeType mime, ScenePin pin);

    public abstract GameObject GenerateSkyPlayer(BaseMimeType mime);
}
