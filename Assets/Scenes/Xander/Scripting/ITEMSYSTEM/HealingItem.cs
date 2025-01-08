using UnityEngine;

public class HealingItem : MonoBehaviour
{
    public void Use()
    {
        Debug.Log("Healing Item Used. +20 HP!");
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.ModifyHealth(true, 20);
        Destroy(gameObject);
    }
}
 