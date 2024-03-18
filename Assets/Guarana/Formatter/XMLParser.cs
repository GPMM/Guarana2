using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;


public class XMLParser
{
    private XmlDocument xmlDoc;
    //private string ncldtd;
    private Dictionary<string, XmlNode> regions, descriptors;
    private Dictionary<string, (EventType, EventTransition)> conditions, actions;

    public XMLParser()
    {
        xmlDoc = new XmlDocument();
        regions = new Dictionary<string, XmlNode>();
        descriptors = new Dictionary<string, XmlNode>();

        conditions = new Dictionary<string, (EventType, EventTransition)>();
        conditions.Add("onBegin", (EventType.PRESENTATION, EventTransition.START));
        conditions.Add("onEnd", (EventType.PRESENTATION, EventTransition.STOP));
        conditions.Add("onAbort", (EventType.PRESENTATION, EventTransition.ABORT));
        conditions.Add("onPause", (EventType.PRESENTATION, EventTransition.PAUSE));
        conditions.Add("onResume", (EventType.PRESENTATION, EventTransition.RESUME));
        conditions.Add("onPrepare", (EventType.PREPARATION, EventTransition.STOP));
        conditions.Add("onEnterView", (EventType.VIEW, EventTransition.START));
        conditions.Add("onExitView", (EventType.VIEW, EventTransition.STOP));

        actions = new Dictionary<string, (EventType, EventTransition)>();
        actions.Add("start", (EventType.PRESENTATION, EventTransition.START));
        actions.Add("stop", (EventType.PRESENTATION, EventTransition.STOP));
        actions.Add("abort", (EventType.PRESENTATION, EventTransition.ABORT));
        actions.Add("pause", (EventType.PRESENTATION, EventTransition.PAUSE));
        actions.Add("resume", (EventType.PRESENTATION, EventTransition.RESUME));
        actions.Add("prepare", (EventType.PREPARATION, EventTransition.START));

        //ncldtd = "< !DOCTYPE ncl360 [  \n" +
        //            "< !ELEMENT region EMPTY>\n" +
        //            "< !ELEMENT descriptor EMPTY>\n" +
        //            "< !ELEMENT media EMPTY>\n" +

        //            "< !ELEMENT head (region*, descriptor+)>\n" +
        //            "< !ELEMENT body (media+)>\n" +
        //            "< !ELEMENT ncl360 (head, body)>\n" +

        //            "< !ATTLIST region id ID #REQUIRED>\n" +
        //            "< !ATTLIST region polar CDATA #REQUIRED>\n" +
        //            "< !ATTLIST region azimuthal CDATA #REQUIRED>\n" +
        //            "< !ATTLIST region radius CDATA #REQUIRED>\n" +
        //            "< !ATTLIST region width CDATA #IMPLIED>\n" +
        //            "< !ATTLIST region height CDATA #IMPLIED>\n" +
        //            "< !ATTLIST region zIndex CDATA #IMPLIED>\n" +
        //            "< !ATTLIST region pin CDATA #IMPLIED>\n" +

        //            "< !ATTLIST descriptor id ID #REQUIRED>\n" +
        //            "< !ATTLIST descriptor region IDREF #REQUIRED>\n" +
        //            "< !ATTLIST descriptor soundType CDATA #IMPLIED>\n" +
        //            "< !ATTLIST descriptor volume CDATA #IMPLIED>\n" +
        //            "< !ATTLIST descriptor dur CDATA #IMPLIED>\n" +

        //            "< !ATTLIST media id ID #REQUIRED>\n" +
        //            "< !ATTLIST media src CDATA #REQUIRED>\n" +
        //            "< !ATTLIST media descriptor IDREF #REQUIRED>\n" +
        //         "]>\n";
    }


    public Document Parse(string xmldocfile, string docid)
    {
        xmlDoc.LoadXml(xmldocfile);
        Document doc = new Document(docid);

        foreach (XmlNode node in xmlDoc.GetElementsByTagName("region"))
        {
            regions.Add(node.Attributes["id"].InnerText, node);
        }

        foreach (XmlNode node in xmlDoc.GetElementsByTagName("descriptor"))
        {
            descriptors.Add(node.Attributes["id"].InnerText, node);
        }

        foreach (XmlNode node in xmlDoc.GetElementsByTagName("media"))
        {
            Media media = ParseMedia(node);
            doc.AddMedia(media);
        }

        foreach (XmlNode node in xmlDoc.GetElementsByTagName("port"))
        {
            ParsePort(node, doc);
        }

        foreach (XmlNode node in xmlDoc.GetElementsByTagName("link"))
        {
            ParseLink(node, doc);
        }

        return doc;
    }


    private Media ParseMedia(XmlNode node)
    {
        Media media = new Media(node.Attributes["id"].InnerText);
        media.SetSrc(node.Attributes["src"].InnerText);

        string descriptor = node.Attributes["descriptor"].InnerText;
        ParseDescriptor(descriptor, media);

        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
        foreach (XmlNode area in node.ChildNodes)
        {
            Area a = new Area(area.Attributes["id"].InnerText);

            try
            {
                string aux = area.Attributes["begin"].InnerText;
                float num = float.Parse(Regex.Match(aux, @"([-+]?[0-9]*\.?[0-9]+)").Groups[1].Value, NumberStyles.Any, ci);
                a.SetBegin(num);
            }
            catch { }

            try
            {
                string aux = area.Attributes["end"].InnerText;
                float num = float.Parse(Regex.Match(aux, @"([-+]?[0-9]*\.?[0-9]+)").Groups[1].Value, NumberStyles.Any, ci);
                a.SetEnd(num);
            }
            catch { }

            media.AddArea(a);
        }

        return media;
    }


    private void ParseDescriptor(string id, Media media)
    {
        XmlNode node = descriptors[id];

        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";

        try
        {
            media.SetSoundType(node.Attributes["soundType"].InnerText);
        }
        catch { }

        try
        {
            string aux = node.Attributes["volume"].InnerText;
            media.SetVolume(float.Parse(aux, NumberStyles.Any, ci));
        }
        catch { }

        try
        {
            string aux = node.Attributes["projection"].InnerText;
            media.SetProjection((ProjectionType)Enum.Parse(typeof(ProjectionType), aux, true));
        }
        catch { }

        try
        {
            string aux = node.Attributes["dur"].InnerText;
            media.SetDur(float.Parse(aux, NumberStyles.Any, ci));
        }
        catch { }

        string region = node.Attributes["region"].InnerText;
        ParseRegion(region, media);
    }


    private void ParseRegion(string id, Media media)
    {
        if (id == "default.sky")
        {
            media.SetInSky();
            return;
        }

        XmlNode node = regions[id];

        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
        string aux;

        aux = node.Attributes["polar"].InnerText;
        media.SetPolar(float.Parse(Regex.Match(aux, @"([-+]?[0-9]*\.?[0-9]+)").Groups[1].Value, NumberStyles.Any, ci));

        aux = node.Attributes["azimuthal"].InnerText;
        media.SetAzimuthal(float.Parse(Regex.Match(aux, @"([-+]?[0-9]*\.?[0-9]+)").Groups[1].Value, NumberStyles.Any, ci));

        aux = node.Attributes["radius"].InnerText;
        media.SetRadius(float.Parse(Regex.Match(aux, @"([-+]?[0-9]*\.?[0-9]+)").Groups[1].Value, NumberStyles.Any, ci));
        
        try
        {
            aux = node.Attributes["width"].InnerText;
            media.SetWidth(float.Parse(Regex.Match(aux, @"([-+]?[0-9]*\.?[0-9]+)").Groups[1].Value, NumberStyles.Any, ci));
        }
        catch { }

        try
        {
            aux = node.Attributes["height"].InnerText;
            media.SetHeight(float.Parse(Regex.Match(aux, @"([-+]?[0-9]*\.?[0-9]+)").Groups[1].Value, NumberStyles.Any, ci));
        }
        catch { }

        try
        {
            aux = node.Attributes["zIndex"].InnerText;
            media.SetZIndex(int.Parse(Regex.Match(aux, @"([-+]?[0-9]*\.?[0-9]+)").Groups[1].Value, NumberStyles.Any, ci));
        }
        catch { }

        try
        {
            aux = node.Attributes["pin"].InnerText;
            media.SetPin((ScenePin)Enum.Parse(typeof(ScenePin), aux, true));
        }
        catch { }
    }


    private void ParsePort(XmlNode node, Document doc)
    {
        string nodeid = node.Attributes["media"].InnerText;
        if (doc.HasMedia(nodeid))
        {
            doc.AddPort(new Action(nodeid, EventType.PRESENTATION, EventTransition.START));
        }
    }


    private void ParseLink(XmlNode node, Document doc)
    {
        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";

        Transition cond = null;
        List<Action> acts = new List<Action>();
        bool hasCondition = false;

        foreach (XmlNode bind in node.ChildNodes)
        {
            string nodeid, trigger, ifaceid;
            float delay;

            nodeid = bind.Attributes["component"].InnerText;
            if (!doc.HasMedia(nodeid))
                continue;

            trigger = bind.Attributes["role"].InnerText;

            try
            {
                ifaceid = bind.Attributes["interface"].InnerText;
            }
            catch
            {
                ifaceid = null;
            }

            try
            {
                string aux = bind.Attributes["delay"].InnerText;
                delay = float.Parse(Regex.Match(aux, @"([-+]?[0-9]*\.?[0-9]+)").Groups[1].Value, NumberStyles.Any, ci);
            }
            catch
            {
                delay = 0f;
            }

            
            // Test if bind is condition
            if (conditions.ContainsKey(trigger) && !hasCondition)
            {
                hasCondition = true;

                cond = new Transition(nodeid, ifaceid, conditions[trigger].Item1, conditions[trigger].Item2);
            }

            // Test if bind is action
            if (actions.ContainsKey(trigger))
            {
                acts.Add(new Action(nodeid, actions[trigger].Item1, actions[trigger].Item2, delay));
            }

        }

        if (cond != null && acts.Count > 0)
        {
            doc.AddLink(cond, acts);
        }
    }
}
