using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerCommunication {

    private string URL;
    private MonoBehaviour monoBehaviour;        // per poter chiamare StartCoroutine
    private Action<UnityWebRequest> callback;   // callback con cui inviare il risultato della web request
    private Coroutine coroutine;              // per fermare la coroutine, e assicurarsi che ce n'è solo una attiva

    private int secondsToWait = 1;

    public ServerCommunication(MonoBehaviour monoBehaviour, string URL, Action<UnityWebRequest> callback) {
        this.monoBehaviour = monoBehaviour;
        this.URL = URL;
        this.callback = callback;   
    }

    public void setSecondsToWait(int seconds) {
        secondsToWait = seconds;
    }

    public void startRequestingValues() {
        stopRequestingValues();
        coroutine = monoBehaviour.StartCoroutine(getValuesCoroutine(URL));
    }

    public void stopRequestingValues() {
        if (coroutine != null) {
            monoBehaviour.StopCoroutine(coroutine);
        }
        coroutine = null;
    }

    // Chiedi un nuovo valore ogni secondo
    IEnumerator getValuesCoroutine(string url) {
        while (true) {

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();                          // invia la richiesta e aspetta la risposta

            // Manda il risultato al ChartPanelHandler
            callback(request);

            // Wait and then repeat
            yield return new WaitForSeconds(secondsToWait);
        }
    }
}
