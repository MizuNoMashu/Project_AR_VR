using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomObserverEventHandler : DefaultObserverEventHandler {

    private VuforiaTargetsHandler vuforiaTargetsHandler = null;
    private int targetId = -1;

    /*
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }
    */

    protected override void OnTrackingFound() {

        base.OnTrackingFound();         // chiama il metodo corrispondente nella classe che stai estendendo

        Debug.Log("Trovato!");

        if (targetId != -1 && vuforiaTargetsHandler != null) {           // se l'oggetto e' stato inizializzato correttamente
            // avvisa lo scene handler
            vuforiaTargetsHandler.foundTarget(targetId);
        }
    }

    public void setVuforiaSceneHandler(VuforiaTargetsHandler vth) {
        vuforiaTargetsHandler = vth;
    }
    public void setId(int id) {
        targetId = id;
    }
}
