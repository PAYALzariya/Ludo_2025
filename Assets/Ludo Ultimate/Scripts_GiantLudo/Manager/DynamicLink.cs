using System;
#if FIREBASE_SDK
using Firebase.DynamicLinks;
#endif
using UnityEngine;

public class DynamicLink : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if FIREBASE_SDK
        DynamicLinks.DynamicLinkReceived += OnDynamicLink;
#endif
    }

    // Display the dynamic link received by the application.
    void OnDynamicLink(object sender, EventArgs args)
    {
#if FIREBASE_SDK
        var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
        Debug.LogFormat("Received dynamic link {0}",
                        dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
        Application.OpenURL(dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
        DynamicLinks.DynamicLinkReceived -= OnDynamicLink;
#endif
    }

}
