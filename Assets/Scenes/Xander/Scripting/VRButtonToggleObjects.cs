using UnityEngine;
using UnityEngine.InputSystem;

public class VRButtonToggleObjects : MonoBehaviour
{
    [Header("GameObjects to Toggle")]
    public GameObject[] objectsToToggle; // Array of GameObjects to toggle

    [Header("Input Settings")]
    public InputActionProperty toggleButton; // Input Action for the "B" button

    private bool isButtonPressed = false; // Track button state

    private void Update()
    {
        // Check if the "B" button is pressed
        if (toggleButton.action.WasPressedThisFrame())
        {
            if (!isButtonPressed)
            {
                ToggleObjects();
                isButtonPressed = true;
            }
        }

        // Reset button state when released
        if (toggleButton.action.WasReleasedThisFrame())
        {
            isButtonPressed = false;
        }
    }

    private void ToggleObjects()
    {
        if (objectsToToggle == null || objectsToToggle.Length == 0)
        {
            Debug.LogWarning("No objects assigned to toggle!");
            return;
        }

        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(!obj.activeSelf); // Toggle active state
                Debug.Log($"Toggled: {obj.name} to {(obj.activeSelf ? "Active" : "Inactive")}");
            }
        }
    }
}
 