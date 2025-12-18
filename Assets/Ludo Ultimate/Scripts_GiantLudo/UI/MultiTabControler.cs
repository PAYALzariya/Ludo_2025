using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiTabControler : MonoBehaviour
{
    #region Private_Variables
    [SerializeField] protected TabListComponent[] tabListComponents;
    [SerializeField] protected TabListComponent[] tabButtons;

    [SerializeField] private int startingTab = 0;

    [Header("Colors")]
    [SerializeField] private Color activeTabColor;
    [SerializeField] private Color inActiveTabColor;
    private bool OpenedFromExternal = false;
    #endregion

    #region  Unity_Callback
    void OnEnable()
    {
        foreach (TabListComponent item in tabButtons)
        {
            item.item.GetComponent<Button>().onClick.RemoveAllListeners();
            item.item.GetComponent<Button>().onClick.AddListener(() =>
            {
                ChangeTabTo(item.position);
            });
        }
        //ChangeTabTo(startingTab);
        ResetTabs();
    }
    void OnDisable()
    {
        OpenedFromExternal = false;
    }
    #endregion

    #region Public_Methods
    public void OpenTabWithButtonDisabled(List<int> tabs)
    {
        foreach (TabListComponent item in tabButtons)
        {
            Color c = item.item.GetComponentInChildren<Text>().color;
            if (tabs.Contains(item.position))
            {
                c.a = 0.5f;
                item.item.GetComponent<Button>().interactable = false;
                item.item.GetComponentInChildren<Text>().color = c;
            }
            else
            {
                c.a = 1;
                item.item.GetComponent<Button>().interactable = true;
                item.item.GetComponentInChildren<Text>().color = c;
            }
        }

        foreach (TabListComponent item in tabButtons)
        {
            if (item.item.GetComponent<Button>().interactable)
            {
                ChangeTabTo(item.position);
                break;
            }
        }
    }
    public void ChangeTabTo(int tabNumber)
    {
        OpenTab(tabNumber);
        ActiveTabButton(tabNumber);
    }
    public void OpenTabsWithSpecificTab(int tabNumber)
    {
        OpenedFromExternal = true;
        ChangeTabTo(tabNumber);
        this.Open();
    }

    protected virtual void ActiveTabButton(int index)
    {
        foreach (TabListComponent item in tabButtons)
        {
            if (item.position == index)
            {
                item.selectBg.Open();
                item.item.transform.GetComponentInChildren<TextMeshProUGUI>().color = activeTabColor;
            }
            else
            {
                item.selectBg.Close();
                item.item.transform.GetComponentInChildren<TextMeshProUGUI>().color = inActiveTabColor;
            }
        }
    }
    #endregion

    #region Public_Methods
    private void OpenTab(int index)
    {
        foreach (TabListComponent component in tabListComponents)
        {
            if (component.position == index)
            {
                component.item.SetActive(true);
            }
            else
            {
                component.item.SetActive(false);
            }
        }
    }

    private void ResetTabs()
    {
        if (OpenedFromExternal)
            return;

        ChangeTabTo(startingTab);
    }
    #endregion
}

[System.Serializable]
public struct TabListComponent
{
    public int position;
    public GameObject item;
    public Image selectBg;
}