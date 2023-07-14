using System.Collections.Generic;
using System.IO;


public enum BaseMimeType { audio, image, model, text, video, application };


public class MimeTypes
{
    private static Dictionary<string, BaseMimeType> baseMimes = new Dictionary<string, BaseMimeType>
	{
        { ".mp3", BaseMimeType.audio },
        { ".ogg", BaseMimeType.audio },

        { ".jpg", BaseMimeType.image },
        { ".jpeg", BaseMimeType.image },
        { ".png", BaseMimeType.image },

        { ".obj", BaseMimeType.model },

        { ".txt", BaseMimeType.text },

        { ".mpeg", BaseMimeType.video },
        { ".mp4", BaseMimeType.video }
    };

    private static Dictionary<string, string> Mimes = new Dictionary<string, string>
    {
        { ".mp3", "audio/mpeg" },
        { ".ogg", "audio/ogg" },

        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },

        { ".obj", "model/obj" },

        { ".txt", "text/plain" },

        { ".mpeg", "video/mpeg" },
        { ".mp4", "video/mp4" }
    };


    public static BaseMimeType GetBaseMimeType(string src)
    {
        string ext = Path.GetExtension(src);
        if (!baseMimes.ContainsKey(ext))
            return BaseMimeType.application;

        return baseMimes[ext];
    }


    public static string GetMimeType(string src)
    {
        string ext = Path.GetExtension(src);
        if (!Mimes.ContainsKey(ext))
            return "application/octet-stream";

        return Mimes[ext];
    }
}
