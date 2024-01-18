using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInfoItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;

    private string infoName;
    private int value;

    public void SetName(string infoName)
    {
        this.infoName = infoName;
    }

    public void SetValue(int value)
    {
        this.value = value;
    }

    public void Update()
    {
        textMeshPro.text = infoName + " : " + value;
    }
}
