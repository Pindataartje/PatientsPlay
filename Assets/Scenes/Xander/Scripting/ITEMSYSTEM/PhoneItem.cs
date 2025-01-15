using UnityEngine;
using TMPro;

public class PhoneItem : MonoBehaviour
{
    public TextMeshProUGUI phoneText;

    public void Use()
    {
        Debug.Log("Phone Item Used. Checking chamber...");
        Revolver revolver = FindAnyObjectByType<Revolver>();

        string result = revolver.Chambers[revolver.CurrentChamber] ? "LIVE BULLET" : "BLANK BULLET";
        Debug.Log(result);

        // Use TextManager to show and clear the text
        if (TextManager.Instance != null)
        {
            TextManager.Instance.ShowTemporaryText(phoneText, result, 4f);
        }

        Destroy(gameObject);
    }
}
 