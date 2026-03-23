using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;      // assigned to same orientation under player
    [SerializeField] private Transform targetPosition;   // empty object under player to follow    
    float maxLookX = 90f;
    float minLookX = -90f;
    [Header("Camera movement settings")]
    [SerializeField] private float defaultMaxYDistance = 0.05f; // how far camera can move up and down from target position
    private float maxYDistance;
    [SerializeField] private float yMoveSmoothingSpeed = 25f;

    private float xRotation;
    private float yRotation;
    private Vector3 smoothY;
    [HideInInspector] public float rotationBlendWeight = 0f; // 0 = full player, 1 = full scripted
    [HideInInspector] public Quaternion scriptedRotation;
    private InputActionAsset inputActions;
    private InputAction lookAction;
    private static float mouseX; public static float MouseX => mouseX;
    private static float mouseY; public static float MouseY => mouseY;
    private static Transform forward; public static Transform Forward => forward;
    void Start()
    {
        GameManager.DisableCursor();
        maxYDistance = defaultMaxYDistance;
        inputActions = GetComponent<PlayerInput>().actions;
        lookAction = inputActions.FindAction("Look");
        forward = transform;
    }
    private void Update()
    {
        if (GameManager.Paused) return;
        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().dialogueIsPlaying) return;

        mouseX = lookAction.ReadValue<Vector2>().x * 0.1f;
        mouseY = lookAction.ReadValue<Vector2>().y * 0.1f;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLookX, maxLookX);

        // Build input-based rotation
        Quaternion playerRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Blend between scripted and player rotation
        Quaternion finalRotation = Quaternion.Slerp(playerRotation, scriptedRotation, rotationBlendWeight);

        transform.rotation = finalRotation;
        orientation.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        // Follow player target position
        float y = transform.position.y;
        y = Mathf.Clamp(y, targetPosition.position.y - maxYDistance, targetPosition.position.y + maxYDistance);
        smoothY = new Vector3(targetPosition.position.x, y, targetPosition.position.z);
        smoothY = Vector3.Lerp(smoothY, targetPosition.position, Time.deltaTime * yMoveSmoothingSpeed);

        transform.position = smoothY;        
    }
}