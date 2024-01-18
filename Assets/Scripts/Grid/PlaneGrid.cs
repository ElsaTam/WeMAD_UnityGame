using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGrid : MonoBehaviour
{
    public event EventHandler OnMouseOverPlane;

    private void OnMouseOver()
    {
        OnMouseOverPlane?.Invoke(this, EventArgs.Empty);
    }
}
