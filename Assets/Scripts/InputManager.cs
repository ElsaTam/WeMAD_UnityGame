#define USE_NEW_INPUT_SYSTEM

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one InputManager. " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }
    
    



    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public bool IsLeftMouseButtonDownThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.LeftClick.WasPressedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    public float GetCameraZoomAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        if (Input.mouseScrollDelta.y > 0) return -1f;
        else if (Input.mouseScrollDelta.y < 0) return 1f;
        return 0f;
#endif
    }

    public Vector2 GetCameraMoveVector()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        Vector2 inputMoveDir = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.y += +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.y += -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x += -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x += +1f;
        }
        return inputMoveDir;
#endif
    }

    public float GetCameraRotateAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
        if (Input.GetKey(KeyCode.Q)) return 1f;
        if (Input.GetKey(KeyCode.E)) return -1f;
        return 0f;
#endif
    }



}
