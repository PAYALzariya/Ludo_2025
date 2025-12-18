using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LocalGamePlayPanel : MonoTemplate
{
    #region PrivateVariables

    [SerializeField] private ColorSelector selector;

    [SerializeField] private Sprite[] playerColors;
    [SerializeField] private Color[] colorForError;
    [SerializeField] private PlayerDetail[] goPlayerDetails;
    [SerializeField] private Image[] imgPlayerCountButton;

    [SerializeField] private Sprite activeButtonSprite;
    [SerializeField] private Sprite normalButtonSprite;

    private int numbersOfPlayer = 0;
    private int playerSelectingColor = 0;
    private int[] selectedColors;
    #endregion

    #region UnityCallback
    void OnEnable()
    {
        SetPlayerCount(2);
        ResetPlayerInputField();
        //selector.Close();
    }
    #endregion

    #region PublicMethods
    public void SetPlayerCount(int number)
    {
        for (int i = 0; i < goPlayerDetails.Length; i++)
        {
            goPlayerDetails[i].ShowObject(i < number);
        }

        for (int i = 0; i < imgPlayerCountButton.Length; i++)
        {
            imgPlayerCountButton[i].sprite = (i == (number - 2)) ? activeButtonSprite : normalButtonSprite;
        }

        numbersOfPlayer = number;
    }

    public void SelectColor(int playerIndex)
    {
        //selector.OpenColorSelector(playerColors, GetSelectedPlayerColor(), selectedColors[playerIndex], SetPlayerColors);
        playerSelectingColor = playerIndex;
    }
    #endregion

    #region  Public_Methods
    public void CloseButtonTap()
    {
        this.Close();
        Ludo_UIManager.instance.homeScreen.Open();
    }
    public void OpenGameScene()
    {
        /*Debug.Log($"Value of error: {CheckForErrors()}");
        return; */

        if (CheckForErrors()) return;

        GameStaticData.playerCount = numbersOfPlayer;
        GameStaticData.gamesType = GamesType.LocalMultiplayer;

        this.Close();
        GetHomeScreen.Close();
        GetGameScreen.Open();
    }
    #endregion

    #region  PrivateMathods
    private bool CheckForErrors()
    {
        bool b = false;
        List<string> playerNames = new List<string>();

        for (int i = 0; i < goPlayerDetails.Length; i++)
        {
            PlayerDetail pd = goPlayerDetails[i];
            if (pd.IsOpen)
            {
                string playerName = "";
                playerName = pd.playerNameInput.text.Trim();
                if (playerName == null || playerName.Length <= 0)
                {
                    string colorName = Cust_Utility.GetColorName(i);
                    colorName = Cust_Utility.GetColoredString(colorForError[i], colorName.ToUpper());
                    GetMessagePanel.DisplayMessage($"Please Enter Name For {colorName} player");
                    b = true;
                    break;
                }
                playerNames.Add(playerName);
            }
            else
            {
                break;
            }
        }
        GameStaticData.playersName = playerNames;
        return b;
    }

    private int[] GetSelectedPlayerColor()
    {
        //Debug.Log($"Numbers Of Player: {numbersOfPlayer}");

        return selectedColors.Subsequence(0, numbersOfPlayer);
    }

    private void ResetPlayerInputField()
    {
        if (selectedColors == null)
            selectedColors = new int[4];

        for (int i = 0; i < goPlayerDetails.Length; i++)
        {
            PlayerDetail pd = goPlayerDetails[i];
            pd.playerNameInput.text = "";
            pd.colorButton.GetComponent<Image>().sprite = playerColors[i];
            selectedColors[i] = i;
        }

    }

    private void SetPlayerColors(int colorIndex)
    {
        int currentPlayerWithSameColor = Array.FindIndex(selectedColors, x => x == colorIndex);
        int playerSelectingColorsColorIndex = selectedColors[playerSelectingColor];

        if (currentPlayerWithSameColor == playerSelectingColor)
            return;

        goPlayerDetails[playerSelectingColor].colorButton.GetComponent<Image>().sprite = playerColors[colorIndex];
        selectedColors[playerSelectingColor] = colorIndex;

        goPlayerDetails[currentPlayerWithSameColor].colorButton.GetComponent<Image>().sprite = playerColors[playerSelectingColorsColorIndex];
        selectedColors[currentPlayerWithSameColor] = playerSelectingColorsColorIndex;
    }
    #endregion

    [System.Serializable]
    private struct PlayerDetail
    {
        public GameObject mainObj;
        public Button colorButton;
        public TMP_InputField playerNameInput;
        private bool open;

        public void ShowObject(bool b)
        {
            mainObj.SetActive(b);
            open = b;
        }

        public bool IsOpen => open;
    }
}