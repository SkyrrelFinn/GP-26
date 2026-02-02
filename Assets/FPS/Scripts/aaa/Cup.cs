using AA0000;
using UnityEngine;

public class Cup : MonoBehaviour
{
    public Bottle bottleToInteractWith;
    [Range(-3.0f,3.0f)]
    public float fillValue;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        { 
            bottleToInteractWith.ChangeLiquidAmount(fillValue);
        }
    }
}
