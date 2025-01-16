using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndFade : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage; // Assign your UI Image here
    public float fadeDuration = 2.0f; // Time for the fade effect
    public int sceneToLoad = 0; // Scene index to load after fade

    private void Start()
    {
        if (fadeImage == null)
        {
            Debug.LogError("Fade Image is not assigned in GameEndFade!");
        }
        else
        {
            // Ensure the fadeImage starts completely transparent
            Color fadeColor = fadeImage.color;
            fadeColor.a = 0;
            fadeImage.color = fadeColor;
            fadeImage.gameObject.SetActive(false); // Hide it initially
        }
    }

    public void StartFade()
    {
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeImage == null) yield break;

        Debug.Log("Fading to black...");
        fadeImage.gameObject.SetActive(true); // Ensure the image is active
        Color fadeColor = fadeImage.color;
        float timer = 0f;

        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration); // Smoothly interpolate alpha
            fadeColor.a = alpha;
            fadeImage.color = fadeColor;
            yield return null;
        }

        // Ensure the alpha is fully opaque
        fadeColor.a = 1;
        fadeImage.color = fadeColor;

        Debug.Log("Fade complete. Loading or reloading scene...");
        SceneManager.LoadScene(sceneToLoad);
    }
}
