using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using MixedReality.Toolkit.SpatialManipulation;
using Unity.VisualScripting;
using MixedReality.Toolkit.UX;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;
using XCharts.Runtime;
using UnityEngine.UI;
using MixedReality.Toolkit;

public class MenuMain : MonoBehaviour
{
    private GameObject pinButton;

    [Header("Panel to show data")]
    [Tooltip("Prefab of a panel with the DialogWithoutFollow script.")]
    [SerializeField] GameObject panelTextButtonsPrefab;         // prefab pannello dialog
    private GameObject infoPanel;           // pannello dove mostrare temperatura, etc
    private float panelDistanceFromMenu = 0.1f;

    private ChartPanelHandler temperaturePanelHandler = null;
    private string temperaturePanelPrefabName = "TemperaturePanel";
    private string temperatureServerURL = "http://localhost:5000/temperature";
    private bool showingTemperature = false;


    [Header("Instructions scene")]
    [Tooltip("Scene containing the instructions.")]
    [SerializeField] UnityEngine.Object instructionsScene;      // scena con le istruzioni


    private void Start() {
        pinButton = transform.Find("Canvas/Pin/Action Button - Pin").gameObject;
    }




    /* Per aprire e chiudere il menu */
    public void toggleMenu() {
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf) {
            // Fai in modo che il menu ti segue
            gameObject.GetComponent<RadialView>().enabled = true;
            // Untoggle pin
            pinButton.GetComponent<PressableButton>().ForceSetToggled(false, false);    // ForceSetToggled(bool active, bool fireEvents = true)
        }
    }


    /* Pulsante start */
    public void startButton() {
        if (!SceneManager.GetSceneByName(instructionsScene.name).isLoaded) {
            SceneManager.LoadScene(instructionsScene.name, LoadSceneMode.Additive);
        }
        gameObject.SetActive(false);
    }


    /* Pulsante temperatura */
    public void showTemperature() {

        // Se e' la prima volta che clicchi sul pulsante, crea il chart panel handler e il pannello
        if (temperaturePanelHandler == null) {
            temperaturePanelHandler = new ChartPanelHandler(this, temperaturePanelPrefabName, ChartPanelHandler.PANEL_TYPE_TEMPERATURE, temperatureServerURL);

            // Rendi il pannello figlio del menu
            GameObject panel = temperaturePanelHandler.instantiatePanel();
            panel.transform.SetParent(gameObject.transform, false);

            // Sposta il pannello a sinistra del menu
            GameObject menuCanvas = gameObject.GetNamedChild("Canvas");
            RectTransform menuCanvasRT = menuCanvas.GetComponent<RectTransform>();
            float menuCanvasWidth = menuCanvasRT.rect.width * menuCanvasRT.localScale.x;
            temperaturePanelHandler.movePanelHorizontally(-menuCanvasWidth/2 - panelDistanceFromMenu - temperaturePanelHandler.getPanelWidth()/2);

            showingTemperature = true;
            temperaturePanelHandler.showPanel();
        }
        // Altrimenti, semplicemente apri o chiudi il pannello
        else {
            showingTemperature = temperaturePanelHandler.togglePanel();
        }
    }


    /* Pulsante altri dati */
    public void showOtherData() {

        if (showingTemperature) {
            showingTemperature = false;
            temperaturePanelHandler.hidePanel();
        }

        instantiateInfoPanel();

        // Imposta il testo e i pulsanti
        // API: https://learn.microsoft.com/en-us/dotnet/api/mixedreality.toolkit.ux.idialog?view=mrtkuxcore-3.0
        IDialog dialog = infoPanel.GetComponent<IDialog>();
        dialog.SetHeader("Other data");
        dialog.SetBody("So interesting...");
        dialog.SetNeutral("Close", (DialogButtonEventArgs arg) =>
        {
            dialog.Dismiss();
            Destroy(infoPanel);
        });

        //setInfoPanelDistantFromMenu(panelDistanceFromMenu);
        dialog.Show();      // Applica i cambiamenti
    }

    private void instantiateInfoPanel() {
        if (infoPanel) Destroy(infoPanel);

        // Istanzia pannello
        infoPanel = Instantiate(panelTextButtonsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        infoPanel.transform.SetParent(gameObject.transform, false);         // false = non mantenere la posiz corrente
        infoPanel.transform.localPosition = new Vector3(infoPanel.transform.localPosition.x - panelDistanceFromMenu, infoPanel.transform.localPosition.y, infoPanel.transform.localPosition.z);
    }

    private void setInfoPanelDistantFromMenu(float dist) {
        if (!infoPanel) return;

        GameObject menuCanvas = gameObject.GetNamedChild("Canvas");
        GameObject infoPanelCanvas = infoPanel.GetNamedChild("Canvas");

        RectTransform menuCanvasRT = menuCanvas.GetComponent<RectTransform>();
        float menuCanvasWidth = menuCanvasRT.rect.width * menuCanvasRT.localScale.x;

        RectTransform infoPanelCanvasRT = infoPanelCanvas.GetComponent<RectTransform>();
        float infoPanelCanvasWidth = infoPanelCanvasRT.rect.width * infoPanelCanvasRT.localScale.x;
        
        float xPosition = infoPanel.transform.localPosition.x - menuCanvasWidth / 2 - dist - infoPanelCanvasWidth / 2;

        infoPanel.transform.localPosition = new Vector3(xPosition, infoPanel.transform.localPosition.y, infoPanel.transform.localPosition.z);
    }


}


