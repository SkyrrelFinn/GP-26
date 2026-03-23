using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("TRANSFORMS")]
    [SerializeField] private Transform swayHolder;
    [SerializeField] private Transform bobbingHolder;

    [Header("POSITION SWAY")]
    [SerializeField] private float positionSwayAmount = 0.1f;
    [SerializeField] private float maxPositionSwayAmount = 0.05f;
    [SerializeField] private float positionSwaySpeed = 5.0f;
    private Vector3 initialPosition;

    [Header("ROTATION SWAY")]
    [SerializeField] private float swayAmount = 8.0f;
    [SerializeField] private float maxSwayAmount = 15.0f;
    [SerializeField] private float swaySpeed = 6.0f;
    private Quaternion initialRotation;

    [Header("BOBBING")]
    [SerializeField] private float bobbingAmplitude = 0.05f;
    [SerializeField] private float bobbingFrequency = 8.0f;
    private Vector3 initialBobbingPosition;
    private float bobTimer = 0f;


    private void Start()
    {
        initialPosition = swayHolder.localPosition;
        initialRotation = swayHolder.localRotation;
        initialBobbingPosition = bobbingHolder.localPosition;
    }

    private void Update()
    {
        PositionSway();
        RotationSway();
        Bobbing();
    }

    private void PositionSway()
    {
        float targetX = PlayerMovement.MoveInput.x * positionSwayAmount;

        //CLAMP THE POSITION SWAY
        targetX = Mathf.Clamp(targetX, -maxPositionSwayAmount, maxPositionSwayAmount);

        Vector3 targetPosition = initialPosition + new Vector3(targetX, 0, 0);

        //LERP SMOOTHLY TO THE NEW POSITION
        swayHolder.localPosition = Vector3.Lerp(swayHolder.localPosition, targetPosition, Time.deltaTime * positionSwaySpeed);
    }

    private void RotationSway()
    {
        float mouseX = CameraController.MouseX * swayAmount;
        float mouseY = CameraController.MouseY * swayAmount;

        //CLAMP THE SWAY
        mouseX = Mathf.Clamp(mouseX, -maxSwayAmount, maxSwayAmount);
        mouseY = Mathf.Clamp(mouseY, -maxSwayAmount, maxSwayAmount);

        Quaternion targetRotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);  //UP AND DOWN ROTATION
        Quaternion targetRotationY = Quaternion.AngleAxis(mouseX, Vector3.up);      //LEFT AND RIGHT ROTATION
        Quaternion targetRotation = initialRotation * targetRotationX * targetRotationY;

        //LERP SMOOTHLY TO THE NEW ROTATION
        swayHolder.localRotation = Quaternion.Slerp(swayHolder.localRotation, targetRotation, swaySpeed * Time.deltaTime);
    }

    private void Bobbing()
    {
        float inputX = PlayerMovement.MoveInput.x;
        float inputY = PlayerMovement.MoveInput.y;
        bool isMoving = Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputY) > 0.1f;

        Vector3 targetPosition = initialBobbingPosition;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobbingFrequency;
            float bobOffset = Mathf.Sin(bobTimer) * bobbingAmplitude;
            targetPosition.y += bobOffset;
        }
        else
        {
            bobTimer = 0f; //RESET PHASE
        }

        bobbingHolder.localPosition = Vector3.Lerp(bobbingHolder.localPosition, targetPosition, 10.0f * Time.deltaTime);
    }
}