using UnityEngine;
using UnityEngine.InputSystem; // If using the new Input System
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;

public class CustomUITriggerSelector : MonoBehaviour
{
    public XRRayInteractor rayInteractor; // Assign your Right Hand Controller's XRRayInteractor here
    public InputActionReference triggerAction; // Reference to the input action for the front trigger

    private void OnEnable()
    {
        // Enable the input action to ensure it listens for input events
        triggerAction.action.Enable();
    }

    private void Update()
    {
        // Check if the trigger button was pressed this frame
        if (triggerAction.action.WasPerformedThisFrame())
        {
            TrySelectUI();
        }
    }

    private void TrySelectUI()
    {
        if (rayInteractor != null && rayInteractor.hasSelection)
        {
            // Perform a raycast from the controller
            if (rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
            {
                // Get the GameObject the raycast hit and attempt to click it if it's a UI Button
                GameObject hitObject = raycastResult.gameObject;
                if (hitObject != null)
                {
                    // Attempt to trigger the button if it's a UI button
                    var button = hitObject.GetComponent<UnityEngine.UI.Button>();
                    if (button != null)
                    {
                        button.onClick.Invoke();
                    }
                }
            }
        }
    }
}