using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerIsInRange;

    private void Awake()
    {
        playerIsInRange = false;
        visualCue.SetActive(false);
    }

    private void Update()
    {
        if (playerIsInRange)
        {
            visualCue.SetActive(true);
            Debug.Log("This works");
            if (InputManager.GetInstance().GetInteractPressed())
            {
                Debug.Log("This works 2");
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
        }

    }
}
