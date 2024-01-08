using MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Networking;
using UnityEngine.UI;
using XCharts.Runtime;

public class ChartPanelHandler {

    // Campi statici
    public static int PANEL_TYPE_TEMPERATURE = 0;


    // Quanti dati mostrare:
    const int DATA_QUANTITY_MORE = 100;
    const int DATA_QUANTITY_LESS = 20;

    private string panelPrefabName;
    private string panelName;
    private string chartObjectNameInPrefab;

    // Pannello:
    private GameObject panel;
    private GameObject canvas;
    private IChartWrapper chartWrapper;
    private MyDialog dialog;

    private int panelType;

    // SERVER:
    private ServerCommunication server;
    private MonoBehaviour monoBehaviour;      // serve un oggetto MonoBehavior da passare al ServerCommunication
    
    private bool showingMsg = false;
    private bool gettingValues = false;
    

    // Costruttore
    public ChartPanelHandler(MonoBehaviour monoBehaviour, string panelPrefabName, int panelType, string serverURL) {
        this.monoBehaviour = monoBehaviour;
        this.panelPrefabName = panelPrefabName;
        this.panelType = panelType;

        if (panelType == PANEL_TYPE_TEMPERATURE) {
            panelName = "Temperature Panel";
            chartObjectNameInPrefab = "SimplifiedLineChart";
            server = new ServerCommunication(monoBehaviour, serverURL, serverCallback);
        }
    }


    public GameObject instantiatePanel() {
        if (!panel) {

            // Istanzia pannello
            panel =  GameObject.Instantiate(Resources.Load(panelPrefabName, typeof(GameObject)), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            panel.name = panelName;
            panel.SetActive(false);                 // inizializzalo disattivato, poi lo attivi quando e' pronto
            canvas = panel.GetNamedChild("Canvas");

            // Crea chartWrapper
            GameObject chart = canvas.GetNamedChild(chartObjectNameInPrefab);
            if (chart == null) {
                Debug.Log("chart is null!");
            }
            if (panelType == PANEL_TYPE_TEMPERATURE) {
                chartWrapper = new SimplifiedLineChartWrapper(chart, DATA_QUANTITY_LESS, DATA_QUANTITY_MORE);
            }

            // (TEMPORANEO) inizializza grafico con questi valori:
            //List<double> values = new List<double>() { 3d, 12d, 5d, 6d, 1d, 3d, 7d, 10d, 5d, 12d, 6d, 1d, 3d, 7d, 3d, 12d };
            //chartWrapper.addNewValues(values);


            // Imposta pulsanti
            dialog = (MyDialog) panel.GetComponent<IDialog>();

            // Chiedi dati al server quando clicchi il pulsante:
            System.Random rnd = new System.Random();
            dialog.SetPositive("Stop", (DialogButtonEventArgs arg) =>
            {
                //chartWrapper.addNewValue(rnd.Next(0, 15));
                //requestOneValueFromServer();

                if (gettingValues) {
                    stopRequestingValues();
                }
                else {
                    // Cancella valori
                    //chartWrapper.clearData();
                    server.startRequestingValues();
                }
            });


            // (TEMPORANEO) cambia max number of values (alterna tra DATA_QUANTITY_LESS e DATA_QUANTITY_MORE)
            dialog.SetNegative("Show more data", (DialogButtonEventArgs arg) =>
            {
                if (chartWrapper.getNumberOfShownValues() > DATA_QUANTITY_LESS) {
                    chartWrapper.setNumberOfShownValues(DATA_QUANTITY_LESS);
                    dialog.SetNegativeLabel("Show more data");
                }
                else {
                    chartWrapper.setNumberOfShownValues(DATA_QUANTITY_MORE);
                    dialog.SetNegativeLabel("Show less data");
                }
                    
            });
            

            // Pulsante per chiudere il grafico
            dialog.SetNeutral("Close", (DialogButtonEventArgs arg) =>
            {
                hidePanel();
            });

            dialog.Show();      // Applica i cambiamenti
        }
        return panel;
    }

    // Toggle panello
    public bool togglePanel() {
        if (panel.activeSelf) {
            hidePanel();
            return false;
        }
        else {
            showPanel();
            return true;
        }
    }

    // Attiva pannello
    public void showPanel() {
        if (!panel) {
            instantiatePanel();
        }

        panel.SetActive(true);

        // Inizia a chiedere dati al server
        startRequestingValues();
    }

    // Disattiva pannello
    public void hidePanel() {
        // Smetti di ricevere dati dal server
        stopRequestingValues();

        panel.SetActive(false);

        // Rimuovi i valori dal grafico, perche' la prossima volta che lo apri andranno messi valori aggiornati
        chartWrapper.clearData();
    }

    private void showMessage(string msg) {
        dialog.SetBody(msg);
        dialog.Show();
    }

    public float getPanelWidth() {
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        return canvasRT.rect.width * canvasRT.lossyScale.x;
    }
    public void movePanelHorizontally(float dist) {
        panel.transform.localPosition = new Vector3(panel.transform.localPosition.x + dist, panel.transform.localPosition.y, panel.transform.localPosition.z);
    }


    /**** SERVER CONNECTION ****/

    private void startRequestingValues() {
        gettingValues = true;
        server.startRequestingValues();
        dialog.SetPositiveLabel("Stop");
        if (showingMsg) {
            showMessage("");
        }
    }
    private void stopRequestingValues() {
        gettingValues = false;
        server.stopRequestingValues();
        dialog.SetPositiveLabel("Start");
        if (showingMsg) {
            showMessage("");
        }
    }


    private void serverCallback(UnityWebRequest request) {
        
        if (request.result == UnityWebRequest.Result.ConnectionError) {
            Debug.LogError(request.error);
            showMessage("Error: Network connection failed.\nTrying again in 5 seconds.");
            showingMsg = true;
            server.setSecondsToWait(5);
        }
        else {
            string data = request.downloadHandler.text;

            int val = 0;
            if (Int32.TryParse(data, out val)) {
                // Se il parsing ha avuto successo
                if (showingMsg) {
                    showMessage("");
                    showingMsg = false;
                }
                chartWrapper.addNewValue(val);
                server.setSecondsToWait(1);
            }
            else {
                showMessage("Error: Server sent invalid data.\nTrying again in 5 seconds.");
                showingMsg = true;
                server.setSecondsToWait(5);
            }
        }
    }
}
