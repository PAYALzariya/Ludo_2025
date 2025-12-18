using TMPro;
using UnityEngine;

public class FoundChatroomPanel : MonoBehaviour
{
    public Transform foundItemParent;
    public TMP_Text foundTxt;
   
    
   internal void UpdateResultText(string userfound)
    {
         foundTxt.text = userfound + "Result found";
    }
    internal void RemovefoundItem()
    {
        foundTxt.text = string.Empty;
        for (int i = 0; i <foundItemParent.childCount; i++)
        {
            Destroy(foundItemParent.GetChild(i).gameObject);
        }
    }
    void OnDisable()
    {
        RemovefoundItem();
    }

}
