using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChartWrapper  {

    public abstract void addNewValue(double value);

    public abstract void clearData();

    public abstract void setNumberOfShownValues(int numberOfShownValues);

    public abstract int getNumberOfShownValues();

}
