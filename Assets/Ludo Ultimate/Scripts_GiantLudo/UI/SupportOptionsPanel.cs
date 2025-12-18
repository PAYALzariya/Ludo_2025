using UnityEngine;

public class SupportOptionsPanel : MonoBehaviour
{
    public void OpenWhatsapp()
    {
     //   AppsflyerInAppEventsManager.Instance.WhatsAppSupprt(false);
        Application.OpenURL(Ludo_UIManager.instance.assetOfGame.SavedLoginData.whatsappUrl);
    }

    public void CallCustomerCare()
    {
  //      AppsflyerInAppEventsManager.Instance.CallCustomerCare(false);
      //  Application.OpenURL("tel://" + Ludo_Constants.LudoConstants.ContactNumber);
    }
}
