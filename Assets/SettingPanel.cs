
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class LoginUserData
{
    public Image profileimage;
    public Image checkmarkimage;
}
public class SettingPanel : MonoBehaviour
{
    public TMP_Text versionTxt;
    public Toggle musicToggle;
    public Toggle soundToggle;
    public Toggle VibrationToggle;
    public List<LoginUserData> userLoginProfilesList = new List<LoginUserData>();
    void OnEnable()
    {
        versionTxt.text = Application.version.ToString();
        musicToggle.isOn = DataManager.instance.ISmusicOn;
        soundToggle.isOn = DataManager.instance.ISSoundOn;
        VibrationToggle.isOn = DataManager.instance.ISVibration;

    }
    public  void OnSoundcontroltogglesClick(int idnex)
    {
        switch (idnex)
        {
                 case 1:
                {
                    DataManager.instance.ISmusicOn = musicToggle.isOn;
                    break;
                }
                case 2:
                {
                    DataManager.instance.ISSoundOn = soundToggle.isOn;
                    break;
                }
                case 3:
                {
                    DataManager.instance.ISVibration = VibrationToggle.isOn;
                    break;
                }
                
        }
        
        
    }
}
