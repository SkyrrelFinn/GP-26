using System;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    public Slider healthSlider;
    private static int currentHealth;
    public int maxHealth;
    public static Action<int, int> onTakeDamage;    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthSlider.value = currentHealth / maxHealth;
        onTakeDamage?.Invoke(currentHealth, damage);
    }
}
