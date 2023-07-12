using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ScenePin { ENVIRONMENT, CAMERA };

public class Media
{

    private string id, src, soundType;
    private float volume, dur, polar, azimuthal, radius, width, height;
    private int zIndex;
    private ScenePin pin;
    private bool inSky;


    public Media(string id)
    {
        this.id = id;

        //set default values for attributes
        soundType = "2D";
        volume = 0;
        width = 0;
        height = 0;
        zIndex = 0;
        pin = ScenePin.ENVIRONMENT;
        inSky = false;
    }


    public string GetId() { return id; }

    public void SetSrc(string src) { this.src = src; }

    public void SetSoundType(string soundType) { this.soundType = soundType; }

    public void SetVolume(float volume) { this.volume = volume; }

    public void SetDur(float dur) { this.dur = dur; }

    public void SetPolar(float polar) { this.polar = polar; }

    public void SetAzimuthal(float azimuthal) { this.azimuthal = azimuthal; }

    public void SetRadius(float radius) { this.radius = radius; }

    public void SetWidth(float width) { this.width = width; }

    public void SetHeight(float height) { this.height = height; }

    public void SetZIndex(int zIndex) { this.zIndex = zIndex; }

    public void SetPin(ScenePin pin) { this.pin = pin; }

    public void SetInSky() { inSky = true; }


    public override string ToString()
    {
        string ret = "(" + id + ")";

        ret += " src:" + src;
        ret += " soundType:" + soundType;
        ret += " volume:" + volume;
        ret += " dur:" + dur;
        ret += " volume:" + volume;
        ret += " polar:" + polar;
        ret += " azimuthal:" + azimuthal;
        ret += " radius:" + radius;
        ret += " width:" + width;
        ret += " height:" + height;
        ret += " zIndex:" + zIndex;
        ret += " pin:" + pin;
        return ret;
    }
}
