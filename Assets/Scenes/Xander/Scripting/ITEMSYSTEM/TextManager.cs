using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowTemporaryText(TextMeshProUGUI textElement, string message, float duration)
    {
        StartCoroutine(DisplayTextRoutine(textElement, message, duration));
    }

    private System.Collections.IEnumerator DisplayTextRoutine(TextMeshProUGUI textElement, string message, float duration)
    {
        textElement.text = message;
        yield return new WaitForSeconds(duration);
        textElement.text = ""; // Clear the text
    }
}
