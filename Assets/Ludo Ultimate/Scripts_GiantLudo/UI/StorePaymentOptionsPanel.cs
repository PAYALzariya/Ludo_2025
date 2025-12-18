using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP;
using System;

public class StorePaymentOptionsPanel : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    #endregion

    #region PRIVATE_VARIABLES
    [SerializeField] private Button btnClosePopup;
    [SerializeField] private RectTransform rectTransformPanel;
    [SerializeField] private RectTransform rectTransformPopup;

    private bool isPopupOpened = false;    
    private Vector3 popupPositionClose;
    private Vector3 popupPositionOpen;
    private float animationTime = 0.25f;
    
    private string upiPayload = "", paymentLink = "";    
    #endregion

    #region DELEGATE_CALLBACKS
    private void Awake()
    {
        rectTransformPopup.sizeDelta = new Vector2(rectTransformPanel.rect.size.x, rectTransformPopup.rect.size.y);
        popupPositionClose = new Vector3(0, -(rectTransformPanel.rect.size.y / 2) - rectTransformPopup.sizeDelta.y, 0);
        popupPositionOpen = new Vector3(0, -(rectTransformPanel.rect.size.y / 2), 0);
    }

    private void OnEnable()
    {
        ResetPopup();    
    }

    private void OnDisable()
    {
        ResetPopup();
    }
    #endregion    

    #region PUBLIC_METHODS
    public void OpenPanel()
    {
        this.Open();
        StartCoroutine(MoveObjectSmoothly(rectTransformPopup.transform, popupPositionClose, popupPositionOpen, animationTime));

        this.paymentLink = "";
        this.upiPayload = "";
    }
    
    public void SetData(string paymentLink, string upiPayload)
    {        
        this.paymentLink = paymentLink;
        this.upiPayload = upiPayload;        
    }

    public void ClosePanel(bool forcefully = false)
    {
        if(isPopupOpened || forcefully)
            StartCoroutine(MoveObjectSmoothly(rectTransformPopup.transform, rectTransformPopup.transform.localPosition, popupPositionClose, animationTime, true));
    }

    public void OpenUpiOption()
    {
        Invoke("OpenUPIPayload", animationTime);
        isPopupOpened = true;
        ClosePanel();
    }

    public void OpenCardBankOption()
    {                
        Invoke("OpenPaymentLink", animationTime);
        isPopupOpened = true;
        ClosePanel();
    }
    #endregion

    #region PRIVATE_METHODS
    private void ResetPopup()
    {
        isPopupOpened = false;
        rectTransformPopup.transform.localPosition = popupPositionClose;
    }
    
    private void OpenUPIPayload()
    {
        Application.OpenURL(upiPayload);
        Ludo_UIManager.instance.homeScreen.Open();
      //  Ludo_UIManager.instance.storeScreen.Close();        
    }

    private void OpenPaymentLink()
    {
        Ludo_UIManager.instance.shopScreen.SetData(paymentLink);
    }
    #endregion

    #region COROUTINE
    private IEnumerator MoveObjectSmoothly(Transform obj, Vector3 fromPos, Vector3 toPos, float time, bool closePopup = false)
    {
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * (1 / time);
            if (obj != null && obj.gameObject.activeInHierarchy)
            {
                obj.localPosition = Vector3.Lerp(fromPos, toPos, i);
            }
            yield return 0;
        }

        if (closePopup)
            this.Close();
        else
            isPopupOpened = true;
    }
    #endregion
}
