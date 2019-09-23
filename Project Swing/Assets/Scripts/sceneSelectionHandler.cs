using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneSelectionHandler : MonoBehaviour
{
    private static sceneSelectionHandler _instance;

    public static sceneSelectionHandler Instance
    {
        get { return _instance; }
    }

    // singleton
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public int gameModeSelected;
}
