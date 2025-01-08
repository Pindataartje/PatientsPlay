using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VRButtonLoadScene : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Scene index to load when pressing the A button.")]
    public int sceneIndex = 0; // Default to Scene 0

    [Header("Input Settings")]
    [Tooltip("Input action for the A button on the right VR controller.")]
    public InputActionProperty loadSceneButton; // Input Action for the "A" button

    private bool isButtonPressed = false; // Track button state

    private void Update()
    {
        // Check if the "A" button is pressed
        if (loadSceneButton.action.WasPressedThisFrame())
        {
            if (!isButtonPressed)
            {
                LoadScene();
                isButtonPressed = true;
            }
        }

        // Reset button state when released
        if (loadSceneButton.action.WasReleasedThisFrame())
        {
            isButtonPressed = false;
        }
    }

    private void LoadScene()
    {
        Debug.Log($"Loading Scene {sceneIndex}...");
        SceneManager.LoadScene(sceneIndex);
    }
}
 