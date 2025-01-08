using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public int sceneIndexToLoad = 0; // Scene index to load

    public void LoadScene()
    {
        Debug.Log($"Loading Scene {sceneIndexToLoad}...");
        SceneManager.LoadScene(sceneIndexToLoad);
    }
}
