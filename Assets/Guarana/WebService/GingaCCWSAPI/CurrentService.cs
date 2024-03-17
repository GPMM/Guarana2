using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CurrentService
{
    private GingaCCWSAPI api;
    private string ApiURL;

    public UserAPI UserAPI;

    public CurrentService(GingaCCWSAPI api)
    {
        this.api = api;
        ApiURL = api.BaseURL + "/dtv/current-service";
        UserAPI = new UserAPI(api);
    }


    public void SetBindToken(string BindToken)
    {
        api.BindToken = BindToken;
    }


    /// <summary> API: 8.3.7
    /// <para>
    ///     Permite acessar a estrutura e o conteúdo de arquivos transmitidos no
    ///     carrossel DSM-CC de uma aplicação Ginga.
    /// </para>
    /// </summary>
    /// <param name="appId">Identificador da aplicação Ginga sendo executada</param>
    /// <param name="path">Caminho do diretório ou arquivo que se deseja acessar,
    /// a partir do diretório-base da aplicação. O uso do caractere ‘/’ no início
    /// do path é opcional, e mesmo com seu uso deve ser retornado o path a partir
    /// do diretório-base.</param>
    /// <returns> Objeto do arquivo obtido no servidor.
    /// <para>
    ///     Se path for um arquivo, o retorno é o payload (conteúdo) deste arquivo.
    ///     Neste caso, o valor do cabeçalho Content-Type deve ser definido de acordo
    ///     com o MIME type do arquivo (derivado a partir de sua extensão, levando
    ///     em conta os MIME types suportados pelo Ginga, conforme definidos em ABNT NBR 15606-1).
    /// </para>
    /// <para>
    ///     Não sendo possível determinar um MIME type específico, a implementação
    ///     do Ginga CC WebServices deve utilizar o MIME type “application/octet-stream”.
    /// </para>
    /// </returns>
    public IEnumerator GetAppsFiles<T>(string appId, string path, Action<T> callback)
    {
        string url = ApiURL + "/apps/" + appId + "/files?path=" + path;
        
        UnityWebRequest wr;
        if (typeof(T) == typeof(string))
        {
            wr = UnityWebRequest.Get(url);
        }
        else if (typeof(T) == typeof(Texture2D))
        {
            wr = UnityWebRequestTexture.GetTexture(url);
        }
        else if (typeof(T) == typeof(AudioClip))
        {
            string ext = Path.GetExtension(path);
            AudioType atype;

            if (ext == ".mp3") atype = AudioType.MPEG;
            else if (ext == ".ogg") atype = AudioType.OGGVORBIS;
            else atype = AudioType.UNKNOWN;

            wr = UnityWebRequestMultimedia.GetAudioClip(url, atype);
        }
        else
        {
            throw new APIException("Unsupported type");
        }

        api.SetCurrentServiceHeaders(wr);
        yield return api.CreateDownloadCoroutine<T>(wr, callback);
    }
}
