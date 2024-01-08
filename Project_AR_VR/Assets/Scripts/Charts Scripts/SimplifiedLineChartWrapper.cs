using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class SimplifiedLineChartWrapper : IChartWrapper {

    private int numberOfShownValues;

    private List<double> savedValues;
    private int maxSavedValues;

    private GameObject chartObject;
    private SimplifiedLineChart chart;



    // Costruttore a cui passi gia' il GameObject chartObject istanziato
    public SimplifiedLineChartWrapper(GameObject chartObject, int numberOfShownValues = 20, int maxSavedValues = 100, string title = null, List<double> values = null) {

        this.numberOfShownValues = numberOfShownValues;
        this.maxSavedValues = maxSavedValues;
        this.chartObject = chartObject;
        this.chart = chartObject.GetComponent<SimplifiedLineChart>();

        // Crea lista di valori salvati
        this.savedValues = new List<double>();
        this.savedValues.Capacity = maxSavedValues;

        // Parti da un grafico vuoto:
        chart.RemoveAllSerie();
        SimplifiedLine serie =  chart.AddSerie<SimplifiedLine>();
        chart.ClearComponentData();

        // Imposta stile
        serie.lineStyle.color = new Color(232f / 255f, 240f / 255f, 253f / 255f);      // Rendi linea grafico bianca (se no viene celeste anche se nel prefab e' bianca)
        serie.animation.enable = false;
        changeScale(0.4f);

        // Inizializza le X:
        resetXAxis();
        setSubTitle(numberOfShownValues + " most recent values");

        if (title != null) setTitle(title);
        if (values != null) addNewValues(values);

    }


    /******************** FUNZIONI DATI ********************/


    // Aggiungi un singolo valore
    public void addNewValue(double value) {

        // Aggiungi il valore al grafico
        chart.GetSerie(0).AddYData(value);
        while (chart.GetSerie(0).data.Count >= numberOfShownValues) {
            chart.GetSerie(0).RemoveData(0);
        }

        // Salva il valore nella lista di valori salvati
        savedValues.Add(value);
        while (savedValues.Count >= maxSavedValues) {
            savedValues.RemoveAt(0);
        }
    }

    // Aggiungi piu' valori
    public void addNewValues(List<double> values) {

        // Re-imposta l'asse x
        resetXAxis();

        // Aggiungi i nuovi valori:
        for (int i = 0; i < values.Count; i++) {
            addNewValue(values[i]);
        }
    }


    // Cambia quanti valori mostrare
    public void setNumberOfShownValues(int numberOfShownValues) {

        if (numberOfShownValues <= 0) return;

        // Assicurati che non superi il numero massimo di valori accettati dal grafico
        int numberOfShownValuesCorrect = Math.Min(numberOfShownValues, maxSavedValues);

        // Aggiorna la quantita' di valori mostrata
        this.numberOfShownValues = numberOfShownValuesCorrect;

        // Ottieni i valori da mettere:
        List<double> vals = getSavedValues(numberOfShownValuesCorrect);

        // Rimuovi tutti i valori:
        clearData();

        // Aggiungi i valori:
        addNewValues(vals);
        
        // Aggiorna subtitle
        setSubTitle(this.numberOfShownValues + " most recent values");
    }

    // Cancella tutti i valori dall'asse y
    public void clearData() {
        // Svuota grafico
        chart.GetSerie(0).ClearData();
        // Svuota lista valori salvati
        savedValues.Clear();
    }


    // Scrivi sull'asse x valori decrescenti da numberOfShownValues a 0
    private void resetXAxis() {
        chart.ClearComponentData();
        for (int i = numberOfShownValues; i >= 0; i--) {
            chart.AddXAxisData(i.ToString());
        }
    }

    // Ottieni un clone degli ultimi "quantity" dati salvati
    public List<double> getSavedValues(int quantity) {
        if (quantity <= 0) {
            return null;
        }
        else if (quantity < savedValues.Count) {
            return new List<double>(savedValues.GetRange(savedValues.Count - quantity, quantity));               // GetRange (int index, int count)
        }
        else
            return new List<double>(savedValues);
    }

    public int getNumberOfShownValues() {
        return numberOfShownValues;
    }



    /****************** FUNZIONI ESTETICHE ******************/
    public void setTitle(string title) {
        Title t;
        chart.TryGetChartComponent<Title>(out t);
        if (t != null) t.text = title;
    }
    public void setSubTitle(string subtitle) {
        Title t;
        chart.TryGetChartComponent<Title>(out t);
        if (t != null) t.subText = subtitle;
    }
    private void changeScale(float newScale) {
        RectTransform rectTrans = chartObject.GetComponent<RectTransform>();
        rectTrans.localScale = new Vector3(newScale, newScale, newScale);
    }
    private void setLayoutElementWidth(int width) {
        LayoutElement layout = chartObject.GetOrAddComponent<LayoutElement>();
        layout.minWidth = width;
    }
    private void setLayoutElementHeight(int height) {
        LayoutElement layout = chartObject.GetOrAddComponent<LayoutElement>();
        layout.minHeight = height;
    }

    /*
    private void setStyle() {

        // TEMA
        chart.theme.sharedTheme.themeType = ThemeType.Dark;
        chart.theme.transparentBackground = true;

        // SERIE
        SimplifiedLine serie = chart.GetSerie<SimplifiedLine>();
        serie.lineStyle.color = new Color(232f/255f, 240f/255f, 253f/255f);         // Rendi linea della serie bianca (se no e' celeste)
        serie.lineStyle.width = 1.5f;                                               // Spessore linea
        serie.animation.enable = false;                                             // disabilita animazione

        // ASSE Y
        YAxis yaxis = chart.GetChartComponent<YAxis>();
        yaxis.splitLine.lineStyle.color = new Color(134f/255f, 152f/255f, 200f/255f, 48f/255f);     // Rendi le linee orizzontali del grafico celesti mezze trasparenti
        yaxis.splitLine.show = true;

        // GRID COORD
        GridCoord gc = chart.GetChartComponent<GridCoord>();
        gc.showBorder = true;                                                       // mostra il bordo destro del grafico (di default non lo mostra)
    }
    */




}
