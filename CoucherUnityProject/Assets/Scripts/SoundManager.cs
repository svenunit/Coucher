using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    


    //Should be in a Scriptable Object...
    //AudioClips SFX
 
    [Header("Environment SFX")]
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public AudioClip levelClear;

    [Header("Player SFX")]
    public AudioClip player1Dashing;
    public AudioClip player2Dashing;


    public AudioClip player1DamageTaken;
    public AudioClip player2DamageTaken;


    [Header("Enemy SFX")]
    public AudioClip enemyHit;
    public AudioClip enemyStun;
    public AudioClip enemyDeath;


    //AudioClips Music
    [Header("Audio Clips Music")]
    public AudioClip bgMusic;


    //AudioSources
    [Header("Audio Sources")]
    public AudioSource audioSourceMain;
    public AudioSource audioSourceSFXPlayer;
    public AudioSource audioSourceSFXEnemy;
    public AudioSource audioSourceSFXUI;













    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        PlayAudioOnSource(bgMusic, audioSourceMain, 1, 10);
    }

    private void Update()
    {   //Debug
        if (Input.GetKeyDown(KeyCode.F12))
            PlayAudioOnSource(doorOpen,audioSourceMain,0,0);
    }


    //PlayeModes: 0 = OneShot, 1 = Repeat
    public void PlayAudioOnSource(AudioClip audioClip, AudioSource audioSource, int playMode, ulong delay) {
        switch (playMode)
        {
            case 0:
                
                audioSource.PlayOneShot(audioClip);
                break;
            case 1:
                audioSource.clip = audioClip;
                audioSource.Play(delay);
                break;
            default:
                break;
        }
    }
    
    
    
}
