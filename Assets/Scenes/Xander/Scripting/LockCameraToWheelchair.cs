using UnityEngine;

public class LockCameraToWheelchair : MonoBehaviour
{
    public Transform wheelchair; // Reference to the wheelchair's Transform
    public Transform playerCamera; // Reference to the player's camera or XR rig
    private Vector3 initialOffset; // Initial offset between the camera and the wheelchair

    void Start()
    {
        if (wheelchair == null || playerCamera == null)
        {
            Debug.LogError("Wheelchair or Player Camera reference not set!");
            return;
        }

        // Calculate the initial offset
        initialOffset = playerCamera.position - wheelchair.position;
    }

    void LateUpdate()
    {
        if (wheelchair != null && playerCamera != null)
        {
            // Lock the camera to the wheelchair's position while maintaining the offset
            playerCamera.position = wheelchair.position + initialOffset;
        }
    }
}