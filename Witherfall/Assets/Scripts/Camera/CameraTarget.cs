using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTarget : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float lookRangeX = 2f;
    [SerializeField] private float lookRangeY = 2f;
    [SerializeField] private float smoothSpeed = 8f;

    private Vector2 lookInput;
    private Vector3 targetLocalPos;
    private Vector3 velocity;

    void Update()
    {
        targetLocalPos = new Vector3(lookInput.x * lookRangeX, lookInput.y * lookRangeY, 0f);
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetLocalPos, ref velocity, 1f / smoothSpeed);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}
