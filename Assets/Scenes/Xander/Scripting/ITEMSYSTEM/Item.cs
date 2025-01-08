using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        EMP,
        Healing,
        Phone,
        Vacuum
    }

    [Header("Item Settings")]
    public ItemType itemType; // Set the type in the inspector
}
 