using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Buttons : MonoBehaviour
{
    public GameObject[] player;
    PlayerSelect playerSelect;
    Button playButton;
    public TMP_Text twoPlayerWarning;
    public TMP_Text goMode;


    //DEBUG
    public TMP_Text controller1;
    public TMP_Text controller2;



    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            playerSelect= GameObject.Find("PlayerSelect").GetComponent<PlayerSelect>();
            playButton = GameObject.Find("PlayButton").GetComponent<Button>();
            player[0].GetComponent<PlayerInput>().setPlayerNumber(0);
            player[1].GetComponent<PlayerInput>().setPlayerNumber(0);
        }
        

        if (playButton != null && playerSelect != null)
        {
            playButton.interactable = false;
        }
    }

    private void Update()
    {
        controller1.text = Input.GetJoystickNames()[0];
        controller2.text = Input.GetJoystickNames()[1];

        Debug.Log(Input.GetJoystickNames());



        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (playerSelect.player1Assigned && playerSelect.player2Assigned)
            {
                twoPlayerWarning.enabled = false;
                goMode.enabled = true;
                playButton.interactable = true;
            }
        }
    }
    public void ExitButton()
   {
        Application.Quit();
   }

   public void PlayButton()
   {
        player[0].GetComponent<PlayerInput>().setPlayerNumber(1);
        player[1].GetComponent<PlayerInput>().setPlayerNumber(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
       SoundManager.instance.PlayAudioOnSource(SoundManager.instance.bgMusicLevel, SoundManager.instance.audioSourceMain, 1, 0);
    }
    
    public void RetryButton()
    {
        player[0].GetComponent<PlayerInput>().setPlayerNumber(1);
        player[1].GetComponent<PlayerInput>().setPlayerNumber(2);
        SoundManager.instance.PlayAudioOnSource(SoundManager.instance.bgMusicLevel, SoundManager.instance.audioSourceMain, 1, 0);
        SceneManager.LoadScene(1);
    }
}
