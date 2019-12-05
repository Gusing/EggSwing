using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class inputIconHandler : MonoBehaviour
{
    public int input;
    public bool showKeyboard;

    void Start()
    {
        if (!showKeyboard && sceneSelectionHandler.Instance.inputIcons == 2)
        {
            GetComponent<TextMeshProUGUI>().text = "";
        }
        else GetComponent<TextMeshProUGUI>().text = "<sprite=" + (input + 11 * sceneSelectionHandler.Instance.inputIcons) + ">";
    }
    
    void Update()
    {
        
    }
}
