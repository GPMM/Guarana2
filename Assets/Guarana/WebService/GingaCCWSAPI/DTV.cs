using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DTV
{
    private GingaCCWSAPI api;
    private string ApiURL;

    public CurrentService CurrentService;

    public DTV(string hostIP, string hostPort)
    {
        api = new GingaCCWSAPI();
        api.SetBaseUrl(hostIP, hostPort);
        ApiURL = api.BaseURL + "/dtv";
        CurrentService = new CurrentService(api);
    }


    public void SetAccessToken(string AccessToken)
    {
        api.AccessToken = AccessToken;
    }


    public void SetRefreshToken(string RefreshToken)
    {
        api.RefreshToken = RefreshToken;
    }


    public string GetRefreshToken()
    {
        return api.RefreshToken;
    }


    /// <summary> API: 8.1.1
    /// <para>
    ///     Requisita o estado de autorização do cliente (identificado por meio
    ///     de seu clientid), estabelecendo o vínculo inicial entre este e o Ginga
    ///     CC WebServices.
    /// </para>
    /// </summary>
    /// <param name="clientId">Identificador do cliente</param>
    /// <param name="displayName">Nome do cliente exibido no pop-up de autorização do usuário.</param>
    /// <returns> Mensagem do tipo DTVAuthorizeReturn contendo o valor challenge.
    /// <para>
    ///     String aleatória, criptografada com algoritmo AES-128, utilizando a
    ///     chave simétrica estabelecida por meio do método de pareamento, e em
    ///     seguida codificada em base64 usando o alfabeto seguro para URL.
    /// </para>
    /// </returns>
    public IEnumerator GetAuthorize(string clientId, string displayName, Action<DTVAuthorizeReturn> callback)
    {
        string url = ApiURL + "/authorize?pm=qrcode&clientid=" + clientId + "&display-name=" + displayName;
        using (UnityWebRequest wr = UnityWebRequest.Get(url))
        {
            api.SetBaseHeaders(wr);
            yield return api.CreateRequestCoroutine<DTVAuthorizeReturn>(wr, callback);
        }
    }


    /// <summary> API: 8.1.2
    /// <para>
    ///     Requisita o estado de autorização do cliente (identificado por meio
    ///     de seu clientid), estabelecendo o vínculo inicial entre este e o Ginga
    ///     CC WebServices.
    /// </para>
    /// </summary>
    /// <param name="clientId">Identificador do cliente</param>
    /// <param name="challengeResponse">Usado quando um cliente não local faz o seu primeiro
    /// acesso, e ainda não possui um refresh-token. Consiste na resposta ao campo
    /// challenge enviado pelo servidor na API 8.1.1.</param>
    /// <returns> Mensagem do tipo DTVTokenReturn contendo o valor accessToken,
    /// tokenType, expiresIn, refreshToken e serverCert.
    /// </returns>
    public IEnumerator GetTokenWithChallenge(string clientId, string challengeResponse, Action<DTVTokenReturn> callback)
    {
        string url = ApiURL + "/token?clientid=" + clientId + "&challenge-response=" + challengeResponse;
        using (UnityWebRequest wr = UnityWebRequest.Get(url))
        {
            api.SetBaseHeaders(wr);
            yield return api.CreateRequestCoroutine<DTVTokenReturn>(wr, callback);
        }
    }


    /// <summary> API: 8.1.2
    /// <para>
    ///     Requisita o estado de autorização do cliente (identificado por meio
    ///     de seu clientid), estabelecendo o vínculo inicial entre este e o Ginga
    ///     CC WebServices.
    /// </para>
    /// </summary>
    /// <param name="clientId">Identificador do cliente</param>
    /// <param name="refreshToken">Usado quando um cliente não local já fez o seu
    /// primeiro acesso anteriormente e já obteve um refreshToken.</param>
    /// <returns> Mensagem do tipo DTVTokenReturn contendo o valor accessToken,
    /// tokenType, expiresIn, refreshToken e serverCert.
    /// </returns>
    public IEnumerator GetTokenWithRefresh(string clientId, string refreshToken, Action<DTVTokenReturn> callback)
    {
        string url = ApiURL + "/token?clientid=" + clientId + "&refresh-token=" + refreshToken;
        using (UnityWebRequest wr = UnityWebRequest.Get(url))
        {
            api.SetBaseHeaders(wr);
            yield return api.CreateRequestCoroutine<DTVTokenReturn>(wr, callback);
        }
    }


    /// <summary> API: 8.2.1
    /// <para>
    ///     Retorna informações do serviço DTV selecionado no momento, assim como
    ///     de seu transport stream de origem.
    /// </para>
    /// </summary>
    /// <returns> Mensagem do tipo DTVCurrentServiceReturn contendo as informações
    /// do serviço DTV selecionado.
    /// </returns>
    public IEnumerator GetCurrentService(Action<DTVCurrentServiceReturn> callback)
    {
        string url = ApiURL + "/current-service";
        using (UnityWebRequest wr = UnityWebRequest.Get(url))
        {
            api.SetDTVHeaders(wr);
            yield return api.CreateRequestCoroutine<DTVCurrentServiceReturn>(wr, callback);
        }
    }


    public IEnumerator PostRemoteDevice(DTVRemoteDeviceBody msgBody, Action<DTVRemoteDeviceReturn> callback)
    {
        string url = ApiURL + "/remote-device";
        string body = JsonUtility.ToJson(msgBody);
        using (UnityWebRequest wr = UnityWebRequest.Post(url, body, api.PostMessageType))
        {
            api.SetCurrentServiceHeaders(wr);
            yield return api.CreateRequestCoroutine<DTVRemoteDeviceReturn>(wr, callback);
        }
    }


    public IEnumerator DeleteRemoteDevice(string handle, Action<APIMessageBody> callback)
    {
        string url = ApiURL + "/remote-device/" + handle;
        using (UnityWebRequest wr = UnityWebRequest.Delete(url))
        {
            api.SetCurrentServiceHeaders(wr);
            yield return api.CreateRequestCoroutine<APIMessageBody>(wr, callback);
        }
    }
}
