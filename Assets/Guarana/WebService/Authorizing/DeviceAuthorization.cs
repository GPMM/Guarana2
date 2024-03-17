using System;

public class DeviceAuthorization : WSStep
{
    private string clientId = Guid.NewGuid().ToString();

    void Start()
    {
        StartPairing();
    }

    
    void Update()
    {
        if (running) return;

        if (output != null)
        {
            Stop();
        }
    }


    private void StartPairing()
    {
        running = true;

        // First get the user authorization according to the pairing method
        StartCoroutine(dtv.GetAuthorize(clientId, "Guarana Player", CompletePairing));
    }

    private void CompletePairing(DTVAuthorizeReturn pairing)
    {
        // Get the accessToken
        StartCoroutine(dtv.GetTokenWithChallenge(clientId, pairing.challenge, SetupToken));
    }


    private void SetupToken(DTVTokenReturn token)
    {
        dtv.SetAccessToken(token.accessToken);
        dtv.SetRefreshToken(token.refreshToken);

        output = new WSStepIO();
        output.status = WSStepStatus.OK;

        running = false;
    }
}
