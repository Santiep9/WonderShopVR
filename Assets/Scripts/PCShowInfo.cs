using UnityEngine;
using TMPro;

public class PCShowInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text infoUI;
    [SerializeField] string[] infoText = new string[3];
    
    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Roca":
                infoUI.text = infoText[1];
                break;
            case "Muñeca":
                infoUI.text = infoText[2];
                break;
            default:
                infoUI.text = infoText[0];
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        infoUI.text = infoText[0];
    }
}
