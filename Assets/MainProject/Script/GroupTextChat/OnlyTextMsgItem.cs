using UnityEngine;

public class OnlyTextMsgItem : MonoBehaviour
{

    public TMPro.TMP_Text message;
    internal void DisplayMsg(string msg)
    {
        message.text = msg;
    }
    
}
