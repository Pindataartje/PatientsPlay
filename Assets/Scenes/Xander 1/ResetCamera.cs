using UnityEngine;

public class ResetCamera : MonoBehaviour
{
    public Transform cameraRig; // The object whose position needs to be reset
    public Vector3 resetGlobalPosition = new Vector3(0, -0.3014401f, 0); // Desired position in global space

    void Update()
    {
        // Check if the "A" button on the right controller is pressed
        if (Input.GetButtonDown("XRI_Right_PrimaryButton")) // Check for A button
        {
            Debug.Log("A button pressed! Attempting to reset camera position...");
            ResetCameraPosition();
        }
    }

    private void ResetCameraPosition()
    {
        if (cameraRig != null)
        {
            Debug.Log("Resetting camera position in global space...");
            cameraRig.position = resetGlobalPosition; // Reset position in global space
        }
        else
        {
            Debug.LogWarning("Camera rig is not assigned!");
        }
    }
}
