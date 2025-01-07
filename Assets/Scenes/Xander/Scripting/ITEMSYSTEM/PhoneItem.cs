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
        phoneText.text = result;
        Debug.Log(result);

        Destroy(gameObject);
    }
}
