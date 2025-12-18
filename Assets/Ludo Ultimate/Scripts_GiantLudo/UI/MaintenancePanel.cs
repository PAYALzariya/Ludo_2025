using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using I2.Loc;

public class MaintenancePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtMessage;

    private void OnEnable()
    {
        Game.Lobby.OnSocketReconnected += CheckServerMaitenanceStatus;
    }

    private void OnDisable()
    {
        Game.Lobby.OnSocketReconnected -= CheckServerMaitenanceStatus;
    }

    public void OpenPanel(string message)
    {
    //    txtMessage.text = (LocalizationManager.CurrentLanguageCode == "en") ? message : GoogleTranslation.ForceTranslate(message,"en",LocalizationManager.CurrentLanguageCode );
        this.Open();
    }

    public void CheckServerMaitenanceStatus()
    {
        BackgroundEventManager.instance.CheckServerMaitenanceStatus();
    }
}
