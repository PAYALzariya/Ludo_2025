using UnityEngine;
using TMPro;
public class NewUserEnter_MsgItem : MonoBehaviour
{
    public TMP_Text userName;
    internal void DisplayMsg(string newusername)
    {
        userName.text = newusername;
    }
}
