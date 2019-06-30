using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
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
        SceneManager.LoadScene(1);
    }
}
