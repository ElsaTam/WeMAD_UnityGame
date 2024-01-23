using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInfoItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textInfoName;
    [SerializeField] private TextMeshProUGUI textValueSelectedUnit;
    [SerializeField] private TextMeshProUGUI textValueTargetUnit;

    public void SetName(string infoName)
    {
        textInfoName.text = infoName;
    }

    public void SetValueSelectedUnit(string value)
    {
        textValueSelectedUnit.text = value;
    }

    public void SetValueTargetUnit(string value)
    {
        textValueTargetUnit.text = value;
    }

    public void RemoveValueTargetUnit()
    {
        textValueTargetUnit.text = "";
    }
}
