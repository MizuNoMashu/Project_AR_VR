using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServerCommunication {

    public abstract void startRequestingValues();
    public abstract void stopRequestingValues();

    public void setSecondsToWait(int seconds);
}
