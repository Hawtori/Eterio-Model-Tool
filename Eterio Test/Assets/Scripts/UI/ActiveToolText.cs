using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActiveToolText : MonoBehaviour
{
    private TMP_Text textBox;

    public Toggles toggles;
    private string lastSavedState = "";

    private void Start()
    {
        textBox = GetComponent<TMP_Text>();
        lastSavedState = toggles.GetCurrentStateString();
    }

    private void Update()
    {
        if (lastSavedState == toggles.GetCurrentStateString()) return;

        lastSavedState = toggles.GetCurrentStateString();
        textBox.text = "Currently active:\n" + lastSavedState;
    }
}
