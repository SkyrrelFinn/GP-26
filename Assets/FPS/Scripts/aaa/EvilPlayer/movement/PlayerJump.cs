using System;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 3;
    private PlayerMovement playerMovement;
    public static Action<float> onJump;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }
    // private void Update()
    // {
    //     if (PlayerControls.GetKeyDown(ActionType.jump))
    //         OnJumpInput();
    // }

    public void OnJump() => OnJumpInput();
    private void OnJumpInput()
    {
        if (GameManager.Paused) return;
        if(playerMovement == null) return;

        if (JumpCondition())
            Jump();
    }
    private bool JumpCondition()
    {
        if (playerMovement?.IsGrounded() == false) return false;
        if (playerMovement?.OnSlope() == true) return false;
        if (PlayerStamina.exhaustion) return false;

        return true;
    }

    private void Jump()
    {
        playerMovement.Jump(Mathf.Sqrt(jumpHeight * -2.0f * PlayerMovement.gravity));
        onJump?.Invoke(jumpHeight);
    }

    private void HighVelocityLand()
    {
        
    }
}
