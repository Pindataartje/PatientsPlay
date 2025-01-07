using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public bool IsOccupied => transform.childCount > 0;

    public void AddItem(GameObject itemPrefab)
    {
        if (!IsOccupied)
        {
            Instantiate(itemPrefab, transform);
        }
        else
        {
            Debug.LogWarning("Slot is already occupied!");
        }
    }

    public void RemoveItem()
    {
        if (IsOccupied)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }
}
