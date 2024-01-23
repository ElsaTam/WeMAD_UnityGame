using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInteraction : MonoBehaviour
{
    public static UIInteraction Instance { get; private set; }

    [SerializeField] private GameObject canvasGameObject;
    private GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;
    List<RaycastResult> raycastResultList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one UIInteraction. " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        graphicRaycaster = canvasGameObject.GetComponent<GraphicRaycaster>();
        pointerEventData = new PointerEventData(EventSystem.current);
        raycastResultList = new List<RaycastResult>();
    }


    public List<GameObject> GetPointedGUIElementList()
    {
        pointerEventData.position = InputManager.Instance.GetMouseScreenPosition();
        raycastResultList.Clear();
        List<GameObject> gameObjectList = new List<GameObject>();

        graphicRaycaster.Raycast(pointerEventData, raycastResultList);

        foreach(RaycastResult raycastResult in raycastResultList)
        {
            gameObjectList.Add(raycastResult.gameObject);
        }
        return gameObjectList;
    }
}
