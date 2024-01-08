using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class VuforiaTargetsHandler : MonoBehaviour {


    [SerializeField] AssetReference[] targetsAddressables;        // array ORDINATO (rispetto all'ordine degli step) dei prefab addressable dei target
    private GameObject[] targets;                                 // array delle ISTANZE dei target (quindi non i prefab ma i GameObject)

    private int currentFoundTarget = -1;

    
    void Start() {
        // Per gestire le eccezioni nel download dei remote bundle, necessario perche' Addressables non gestisce nulla
        ResourceManager.ExceptionHandler = CustomExceptionHandler;

        // Inizializza array targets della stessa misura di targetsAddressables
        targets = new GameObject[targetsAddressables.Length];

        // Da togliere se facciamo che la funzione viene chiamata da un pulsante "start"
        startTargets();
    }

    public void startTargets() {
        // Scarica e istanzia il primo addressable target
        instantiateAddressableTarget(0);
    }


    public void foundTarget(int targetId) {

        if (currentFoundTarget != targetId) {       // perche' magari avevo gia' trovato questo target e poi l'ho perso e ri-trovato

            // Rimuovi dalla memoria il target precedente a quello appena trovato
            int prevTarget = previousTargetId(targetId);
            if (targets[prevTarget] != null) {
                Addressables.ReleaseInstance(targets[prevTarget]);  // vedi nota in fondo
                targets[prevTarget] = null;
            }

            // Scarica e istanzia il prossimo addressable target
            int nextTarget = nextTargetId(targetId);
            instantiateAddressableTarget(nextTarget);

            currentFoundTarget = targetId;
        }
    }

    private void instantiateAddressableTarget(int id) {

        // Imposta la scena Instructions come scena attiva, altrimenti gli oggetti vengono istanziati nella scena dei menu
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.SetActiveScene(gameObject.scene);

        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(targetsAddressables[id]);
        handle.Completed += (AsyncOperationHandle<GameObject> a) =>
        {
            if (handle.Status == AsyncOperationStatus.Failed) {

                Debug.Log("Remote bundle download failed.");

                /// TODO: dare modo di ri-tentare l'operazione, tramite un pulsante o in modo automatico dopo un tot di tempo
                // ...
            }

            else if (handle.Status == AsyncOperationStatus.Succeeded) {
                // Quando l'hai scaricato passagli il suo id e questo script:
                targets[id] = handle.Result;
                targets[id].GetComponent<CustomObserverEventHandler>().setId(id);
                targets[id].GetComponent<CustomObserverEventHandler>().setVuforiaSceneHandler(this);
            }

            // Rimetti la scena attiva di prima, se nel frattempo non e' stata cambiata da qualcos'altro
            if (SceneManager.GetActiveScene() == gameObject.scene) {
                SceneManager.SetActiveScene(activeScene);
            }
        };
    }


    public void jumpToTarget(int id) {
        // Se id non ha un valore valido non fare nulla
        if (id < 0 || id >= targetsAddressables.Length) {
            return;         // da mettere che mostra un messaggio del tipo "indice non valido. scegliere un valore tra 0 e N"
        }

        // Se il target id e' gia' istanziato non fare nulla
        if (targets[id] != null) {
            return;
        }

        // Elimina tutti i target attuali (se la lista targetsAddressables e' molto lunga bisogna usare
        // una soluzione migliore, tipo tenere traccia in due variabili di quali target sono scaricati
        for (int i = 0; i < targets.Length; i++) {
            if (targets[i] != null) {
                Addressables.ReleaseInstance(targets[i]);
            }
        }
        // Istanzia target 
        instantiateAddressableTarget(id);
        currentFoundTarget = -1;
    }


    private int nextTargetId(int targetId) {
        int targ = targetId + 1;
        if (targ >= targets.Length) targ = 0;

        return targ;
    }

    private int previousTargetId(int targetId) {
        int targ = targetId - 1;
        if (targ < 0) targ = targets.Length - 1;

        return targ;
    }


    void CustomExceptionHandler(AsyncOperationHandle handle, Exception exception) {
        if (exception.HResult == -2146233088)
        {
            Debug.Log("Addressables: Connection error, failed to load remote bundle.");
        }
        else
        {
            Debug.LogWarning("Unhandled Error + " + exception.Message);
        }
    }
}


/* NOTA:
 * Addressables memory management:
 * https://docs.unity3d.com/Packages/com.unity.addressables@1.3/manual/MemoryManagement.html
 * "You can load an asset bundle, or its partial contents, but you cannot partially unload an asset bundle."
 * Quindi se vogliamo evitare di tenere tutti i target in memoria, dobbiamo mettere ogni target
 * in un asset bundle (cioe' addressables group) diverso.
 * In questo modo facendo il release del target dovrebbe eliminare dalla memoria tutto il bundle (spero?)
*/