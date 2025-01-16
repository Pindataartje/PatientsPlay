using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Function to quit the game
    public void QuitApplication()
    {
        Debug.Log("Quit button pressed. Exiting the game...");
        Application.Quit(); // Quits the application
    }
}
