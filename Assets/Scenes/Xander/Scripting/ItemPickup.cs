using UnityEngine;
using UnityEngine.InputSystem; // To use Unity Input System

public class ItemPickup : MonoBehaviour
{
    public Transform leftHand; // Reference to where the item should attach in the left hand
    public float pickupRange = 2f; // Range within which items can be picked up
    public string itemTag = "Item"; // Tag used to identify items that can be picked up
    private GameObject currentItem = null;

    void Update()
    {
        // Cast a ray from the camera forward
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, pickupRange))
        {
            // Check if the object has the tag "Item"
            if (hit.collider.CompareTag(itemTag))
            {
                // When the left trigger is pressed, grab the item
                if (Gamepad.current != null && Gamepad.current.leftTrigger.wasPressedThisFrame)
                {
                    GrabItem(hit.collider.gameObject);
                }
            }
        }
    }

    void GrabItem(GameObject item)
    {
        if (currentItem == null)
        {
            // Set the item to be kinematic and parent it to the hand
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            item.transform.SetParent(leftHand);
            item.transform.localPosition = Vector3.zero;
            currentItem = item;
        }
        else
        {
            // Release the current item if holding one already
            ReleaseItem();
        }
    }

    void ReleaseItem()
    {
        if (currentItem != null)
        {
            // Unparent the item and make it dynamic again
            Rigidbody rb = currentItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            currentItem.transform.SetParent(null);
            currentItem = null;
        }
    }
}
