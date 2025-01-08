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
    }

    public void StartFade()
    {
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeImage == null) yield break;

        Debug.Log("Fading to black...");
        fadeImage.gameObject.SetActive(true);
        Color fadeColor = fadeImage.color;
        float timer = 0f;

        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeColor.a = alpha;
            fadeImage.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 1;
        fadeImage.color = fadeColor;

        Debug.Log("Fade complete. Loading scene...");
        SceneManager.LoadScene(sceneToLoad);
    }
}
