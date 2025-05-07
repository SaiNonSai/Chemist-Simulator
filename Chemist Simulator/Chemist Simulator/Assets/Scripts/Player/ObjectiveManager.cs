using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public TMP_Text ObjectiveText;

    private string currentObjective;

    private void Start()
    {
        SetObjective("Objective: Mix Hydrogen and Oxygen.");

    }

    public void SetObjective(string objective)
    {
        currentObjective = objective;
        ObjectiveText.text = currentObjective; 
    }
}
