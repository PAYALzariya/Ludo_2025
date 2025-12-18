using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
//using I2.Loc;

public class MessagePanel : MonoBehaviour
{
    #region Public_Variables
    #endregion

    #region Private_Variables
    [SerializeField] private RectTransform container;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI message;

    [Space]
    [SerializeField] private Button btnYes;
    [SerializeField] private Button btnNo;
    [SerializeField] private Button btnCancel;

    [Space]
    [SerializeField] float normalTitleHeight = 105f;
    [SerializeField] float hiddenTitleHeight = 20f;
    #endregion

    #region  Unity_Callback

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }
    #endregion

    #region Public_Methods
    public void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.message.GetComponent<RectTransform>());
    }
    private void SetActionForOkButton(Action callback = null)
    {
        btnYes.onClick.RemoveAllListeners();
        btnYes.onClick.AddListener(() =>
        {
            if (callback != null)
                callback();
            this.Close();
        });
    }
    #endregion

    #region Public_Methods

    public void DisplayMessage(string message)
    {
        GetButtonTextComponent(btnYes).text = "Ok";
       // GetButtonTextComponent(btnYes).text = (LocalizationManager.CurrentLanguageCode == "en") ?"Ok":GoogleTranslation.ForceTranslate("Okay","en",LocalizationManager.CurrentLanguageCode);
        this.Open();
        this.message.text = message;//(LocalizationManager.CurrentLanguageCode == "en") ? message : GoogleTranslation.ForceTranslate(message, "en", LocalizationManager.CurrentLanguageCode);
        this.title.text = "";

        SetNormalMessageButtons();
        SetActionForOkButton();
        SetTitleHeight(hiddenTitleHeight);

        RebuildLayout();
        this.Open();
    }

    public void DisplayStaticMessage(string message, Action callback)
    {
        GetButtonTextComponent(btnYes).text = "Ok";
        this.Open();
        this.message.text = $"{message}";
        this.title.text = "";

        SetNormalMessageButtons();
        btnYes.onClick.RemoveAllListeners();
        btnYes.onClick.AddListener(() =>
        {
            if (callback != null)
                callback();
        });
        SetTitleHeight(hiddenTitleHeight);

        RebuildLayout();
        this.Open();
    }

    public void DisplayMessage(string message, Action callback)
    {
        GetButtonTextComponent(btnYes).text = "Ok";
        this.Open();
        this.message.text = $"{message}";
        this.title.text = "";

        SetNormalMessageButtons();
        SetActionForOkButton(callback);
        SetTitleHeight(hiddenTitleHeight);

        RebuildLayout();
        this.Open();
    }

    public void DisplayConfirmationMessage(string message, Action<bool> callback)
    {
        title.text = "Are You Sure?";
        this.message.text = message;
        GetButtonTextComponent(btnYes).text = "Yes";
        GetButtonTextComponent(btnNo).text = "No";

        SetTitleHeight(normalTitleHeight);

        btnYes.onClick.RemoveAllListeners();
        btnYes.onClick.AddListener(() =>
        {
            callback(true);
            this.Close();
        });

        btnNo.onClick.RemoveAllListeners();
        btnNo.onClick.AddListener(() =>
        {
            callback(false);
            this.Close();
        });

        RebuildLayout();
        SetConfirmationButtons();
        this.Open();
    }

    public void DisplayConfirmationMessage(string titleText, string message, string positiveText, string NegativeText, Action<bool> callback)
    {
        title.text = titleText;
        this.message.text = message;
        GetButtonTextComponent(btnYes).text = positiveText;
        GetButtonTextComponent(btnNo).text = NegativeText;

        SetTitleHeight(normalTitleHeight);

        btnYes.onClick.RemoveAllListeners();
        btnYes.onClick.AddListener(() =>
        {
            callback(true);
            this.Close();
        });

        btnNo.onClick.RemoveAllListeners();
        btnNo.onClick.AddListener(() =>
        {
            callback(false);
            this.Close();
        });

        RebuildLayout();
        SetConfirmationButtons();
        this.Open();
    }

    public string ReturnCurrentMessage()
    {
        return this.message.text;
    }

    #endregion

    #region Private_methods

    private void SetConfirmationButtons()
    {
        btnYes.gameObject.SetActive(true);
        btnNo.gameObject.SetActive(true);
        btnCancel.gameObject.SetActive(false);
    }

    private void SetNormalMessageButtons()
    {
        btnYes.gameObject.SetActive(true);
        btnNo.gameObject.SetActive(false);
        btnCancel.gameObject.SetActive(false);
    }
    private TextMeshProUGUI GetButtonTextComponent(Button b)
    {
        return b.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void SetTitleHeight(float newHeight)
    {
        RectTransform rt = title.GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
    }

    private Button GetButtonNumber(int i)
    {
        return transform.GetChild(0).GetChild(0).GetChild(2).GetChild(i).GetComponent<Button>();
    }
    #endregion

    #region Coroutine
    #endregion
}
