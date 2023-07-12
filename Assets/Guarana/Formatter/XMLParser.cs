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

    public XMLParser()
    {
        xmlDoc = new XmlDocument();
        regions = new Dictionary<string, XmlNode>();
        descriptors = new Dictionary<string, XmlNode>();

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


    public Document Parse(string xmldocfile)
    {
        xmlDoc.LoadXml(xmldocfile);
        Document doc = new Document();

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

        return doc;
    }


    private Media ParseMedia(XmlNode node)
    {
        Media media = new Media(node.Attributes["id"].InnerText);
        media.SetSrc(node.Attributes["src"].InnerText);

        string descriptor = node.Attributes["descriptor"].InnerText;
        ParseDescriptor(descriptor, media);

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
}
