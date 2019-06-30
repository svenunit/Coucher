using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Buttons : MonoBehaviour
{
    PlayerSelect playerSelect;
    Button playButton;
    public TMP_Text twoPlayerWarning;
    public TMP_Text goMode;



    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            playerSelect= GameObject.Find("PlayerSelect").GetComponent<PlayerSelect>();
            playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        }

        if (playButton != null && playerSelect != null)
        {
            playButton.interactable = false;
        }
    }

    private void Update()
    {
        if (playerSelect.player1Assigned && playerSelect.player2Assigned)
        {
            twoPlayerWarning.enabled = false;
            goMode.enabled = true;
            playButton.interactable = true;
        }
    }
    public void ExitButton()
   {
        Application.Quit();
   }

   public void PlayButton()
   {
        
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
       SoundManager.instance.PlayAudioOnSource(SoundManager.instance.bgMusicLevel, SoundManager.instance.audioSourceMain, 1, 0);
    }
    
    public void RetryButton()
    {
        SoundManager.instance.PlayAudioOnSource(SoundManager.instance.bgMusicLevel, SoundManager.instance.audioSourceMain, 1, 0);
        SceneManager.LoadScene(1);
    }
}
