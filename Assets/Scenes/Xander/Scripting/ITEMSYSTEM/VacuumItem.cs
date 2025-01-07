using UnityEngine;

public class VacuumItem : MonoBehaviour
{
    public void Use()
    {
        Debug.Log("Vacuum Gun used! (Functionality not yet implemented.)");
        Destroy(gameObject); // Remove the item after use
    }
}
