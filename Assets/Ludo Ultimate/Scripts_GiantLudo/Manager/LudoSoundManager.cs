using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LudoSoundManager : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    public AudioSource Attention;
    public AudioSource DiceClick;
    public AudioSource DiceMovement;
    public AudioSource MyTurn;
    public AudioSource OpponentTurn;
    public AudioSource InHome;
    public AudioSource gameStart;
    public AudioSource StarSafe;
    public AudioSource firstStepSound;
    public AudioSource skipStepSound;
    public AudioSource TokenKill;
    public AudioSource TokenMove;
    public AudioSource Winning;
    public AudioSource bubbleClick;
    public AudioSource Fold;
    public AudioSource newsnd17;
    public AudioSource Raise;
    public AudioSource bgSound;
    public AudioSource WelcometoLudoGiant;
    public Transform Parent;
    public List<Transform> allAudios;
    #endregion

    #region PRIVATE_VARIABLES
    #endregion

    #region UNITY_CALLBACKS
    void OnEnable()
    {
        //		PlayerPrefs.DeleteKey ("Sound");
        /*if(PlayerPrefs.GetInt("enableallprefas", 0) == 0)
		{
			DataManager.IsSoundEnabled = true;
			DataManager.IsMusicEnabled = true;

			PlayerPrefs.GetInt ("enableallprefas", 100);
		}*/
        if (!PlayerPrefs.HasKey("bgMusic"))
        {
            PlayerPrefs.SetInt("bgMusic", 1);
        }
        if (!PlayerPrefs.HasKey("Sound"))
        {
            PlayerPrefs.SetInt("Sound", 1);
        }
        if (!PlayerPrefs.HasKey("Vibration"))
        {
            PlayerPrefs.SetInt("Vibration", 1);
        }
        if (!PlayerPrefs.HasKey("PushNotification"))
        {
            PlayerPrefs.SetInt("PushNotification", 1);
        }

        for (int i = 0; i < Parent.childCount; i++)
        {
            allAudios.Add(Parent.GetChild(i).transform);
        }

        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            foreach (Transform Source in allAudios)
            {
                Source.GetComponent<AudioSource>().enabled = true;
            }
            //PlayBackground ();
        }
        else
        {
            foreach (Transform Source in allAudios)
            {
                Source.GetComponent<AudioSource>().enabled = false;
            }
        }
    }
    #endregion

    #region PUBLIC_METHODS
    /// <summary>
    /// Plays the button click sound.
    /// </summary>
    public void PlayBgSound()
    {
        if (PlayerPrefs.GetInt("bgMusic", 1) == 1 && !bgSound.isPlaying)
        {
            bgSound.Play();
            bgSound.enabled = PlayerPrefs.GetInt("bgMusic", 1) == 0 ? false : true;
        }
    }
    public void stopBgSound()
    {
        bgSound.Stop();
    }
    public void DiceClickSoundOnce()
    {

        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            //    DiceClick.Play();
        }
    }

    public void DiceMovementSoundOnce()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            DiceMovement.Play();
        }
    }
    public void DiceMovementSoundOff()
    {
        DiceMovement.Stop();
    }
    public void MyTurnSoundOnce()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            MyTurn.Play();
        }
    }
    public void OpponentTurnSoundOnce()
    {

        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            OpponentTurn.Play();
        }
    }
    /*public void SpeedButtonClickOnce (bool loop)
	{

		if (PlayerPrefs.GetInt ("SfxMusic", 1) == 1) {
			SpeedAudio.loop = loop;
			if(loop)
				SpeedAudio.Play ();
			else
				SpeedAudio.Stop ();
		}
	}*/
    public void InstarsafeAreaOnce()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            StarSafe.Play();
        }
    }

    public void WelcomeToLudoGiantOnce()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            WelcometoLudoGiant.Play();
        }
    }

    public void FirstStepSound()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            firstStepSound.Play();
        }
    }

    public void SkipStepSound()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            skipStepSound.Play();
        }
    }
    public void InGameStartedOnce()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            print("come ");
            gameStart.Play();
        }
    }
    public void InGameStartedStop()
    {

        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            gameStart.Stop();
        }
    }
    public void AttentionSoundOnce()
    {

        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            Attention.Play();
        }
    }

    public void InHomeSoundOnce()
    {

        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            InHome.Play();
        }
    }
    public void TokenKillSoundOnce()
    {

        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            TokenKill.Play();
        }
    }
    public void TokenMoveSoundOnce()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            TokenMove.Play();
        }
    }

    public void FoldClickOnce()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            Fold.Play();
        }
    }

    public void WinningSoundOnce()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            Winning.Play();
        }
    }

    public void newsnd17ClickOnce()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            newsnd17.Play();
        }
    }
    public void RaiseClickOnce()
    {

        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            Raise.Play();
        }
    }

    public void BubbleClickSound()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
            bubbleClick.Play();
    }

    public void Vibrate()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (PlayerPrefs.GetInt("Vibration", 1) == 1)
        {
            Vibration.Vibrate(60);
            //Handheld.Vibrate();
        }
#endif
    }

    #endregion
}