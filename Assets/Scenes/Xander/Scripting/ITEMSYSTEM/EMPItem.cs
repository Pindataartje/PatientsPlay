using UnityEngine;

public class EMPItem : MonoBehaviour
{
    public void Use()
    {
        Debug.Log("EMP Grenade used! Enemy's next turn will be skipped.");
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.SkipEnemyTurn();
        }
        Destroy(gameObject); // Remove the item after use
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("EMP Grenade hit the enemy! Skipping their next turn.");
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.SkipEnemyTurn();
            }
            Destroy(gameObject); // Remove the item after successful hit
        }
    }
}
