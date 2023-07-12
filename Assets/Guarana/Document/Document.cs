using System.Collections;
using System.Collections.Generic;


public class Document
{
    Dictionary<string, Media> medias;


    public Document()
    {
        medias = new Dictionary<string, Media>();
    }


    public void AddMedia(Media m)
    {
        medias.Add(m.GetId(), m);
    }


    public override string ToString()
    {
        string ret = "";
        int num = medias.Count;
        foreach (KeyValuePair<string, Media> kvp in medias)
        {
            ret += kvp.Value.ToString();
            num--;
            if (num > 0) ret += "\n";
        }
        return ret;
    }
}
