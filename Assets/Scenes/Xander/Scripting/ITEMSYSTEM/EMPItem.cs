using UnityEngine;

public class EMPItem : MonoBehaviour
{
    public void Use()
    {
        Debug.Log("EMP Grenade thrown!");

        // Activate EMP Grenade physics for throwing
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(transform.forward * 500f); // Adjust force as needed
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("EMP Grenade hit Enemy. Enemy skips next turn.");
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            gameManager.SkipEnemyTurn();
            Destroy(gameObject); // Remove the item after use
        }
    }
}
