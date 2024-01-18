using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private CinemachineTransposer cinemachineTransposer;
    private float targetFollowOffsetY;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffsetY = cinemachineTransposer.m_FollowOffset.y;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();
        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        float moveSpeed = 10f;
        transform.position += moveSpeed * Time.deltaTime * moveVector;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, InputManager.Instance.GetCameraRotateAmount(), 0);
        float rotationSpeed = 100f;
        transform.eulerAngles += rotationSpeed * Time.deltaTime * rotationVector;
    }

    private void HandleZoom()
    {
        targetFollowOffsetY += InputManager.Instance.GetCameraZoomAmount();
        targetFollowOffsetY = Mathf.Clamp(targetFollowOffsetY, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float zoomSpeed = 5f;
        cinemachineTransposer.m_FollowOffset.y = Mathf.Lerp(cinemachineTransposer.m_FollowOffset.y, targetFollowOffsetY, Time.deltaTime * zoomSpeed);
    }
}
