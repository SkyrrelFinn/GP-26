using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    public Slider staminaSlider;
    private static float currentStamina;
    public float maxStamina;
    public float staminaLossWhenSprinting;
    public float jumpStaminaLoss;
    public float staminaRecovery;
    public float exhaustionDuration;
    public static bool exhaustion {private set; get;}
    private bool canRecover = true;
    public static float GetStamina => currentStamina;    
    private void Start() {
        currentStamina = maxStamina;
        IncreaseStamina(0);
    }
    void Awake()
    {
        PlayerJump.onJump += ReduceStaminaOnJump;
    }
    void OnDestroy()
    {
        PlayerJump.onJump -= ReduceStaminaOnJump;
    }
    private void ReduceStaminaOnJump(float _) => IncreaseStamina(-jumpStaminaLoss);
    public void IncreaseStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaSlider.value = currentStamina / maxStamina;
        if(currentStamina == 0)
            StartCoroutine(ExhaustionCoroutine());
    }

    private void Update() {

        if(!canRecover) return;
        IncreaseStamina((PlayerMovement.Sprinting && PlayerMovement.MoveInput != Vector2.zero
         ? -staminaLossWhenSprinting : staminaRecovery) * Time.deltaTime);
    }

    private IEnumerator ExhaustionCoroutine()
    {
        exhaustion = true;
        canRecover = false;
        yield return new WaitForSeconds(1f);
        canRecover = true;
        yield return new WaitForSeconds(exhaustionDuration - 1f);
        exhaustion = false;
    }
}
