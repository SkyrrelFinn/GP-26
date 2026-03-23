using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerView : MonoBehaviour
{
    public Player player;
    public InputActionAsset inputActions;    
    public float checkDistance;
    public LayerMask ignoredLayers;
    private RaycastHit hit;
    private IInteractable interactable;
    private Transform currentObj;
    public static LayerMask IgnoredPlayerLayers;
    private void OnEnable()
    {
        inputActions.FindAction("Interact").performed += _ => TryInteract();
        IgnoredPlayerLayers = ignoredLayers;
    }
    private void OnDisable() {
        inputActions.FindAction("Interact").performed -= _ => TryInteract();
    }
    private void Update()
    {
        if(GameManager.Paused) return;

        CheckForInteractables();                    
    }
    private void TryInteract()
    {        
        if(interactable != null)
                interactable.Interact(player);     
    }

    private bool CheckForInteractables()
    {    
        if(!Physics.Raycast(transform.position, transform.forward, out hit, checkDistance, ~ignoredLayers)) return InvalidItem();
        if(hit.transform == currentObj) return false; //Don't reset interactable, just don't proceed to component check

        if(currentObj != null)if(currentObj.GetComponentInChildren<Outline>()) currentObj.GetComponentInChildren<Outline>().enabled = false;

        currentObj = hit.transform;

        if(currentObj != null)if(currentObj.GetComponentInChildren<Outline>()) currentObj.GetComponentInChildren<Outline>().enabled = true;

        IInteractable newInteractable = hit.transform.GetComponentInChildren<IInteractable>();
        if(newInteractable == null) return InvalidItem();

        UpdateNewInteractable(newInteractable);
        return true;
    }

    private bool InvalidItem()
    {
        if(currentObj != null)if(currentObj.GetComponentInChildren<Outline>()) currentObj.GetComponentInChildren<Outline>().enabled = false;
        currentObj = null;
        if(interactable != null)
            UpdateNewInteractable(null);

        interactable = null;        
        return false;
    }

    private void UpdateNewInteractable(IInteractable interactable)
    {
        this.interactable = interactable;
    }
}
