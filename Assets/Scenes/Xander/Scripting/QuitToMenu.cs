using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class QuitToMenu : MonoBehaviour
{
    [Header("Menu Settings")]
    public int menuSceneIndex = 0; // Scene index for the main menu

    private void Update()
    {
        // Check if the "Y" button (XRI_Left_SecondaryButton) is pressed
        if (Input.GetButtonDown("XRI_Left_SecondaryButton"))
        {
            Debug.Log("Y button pressed on left controller. Returning to menu...");
            SceneManager.LoadScene(menuSceneIndex);
        }
    }
}
