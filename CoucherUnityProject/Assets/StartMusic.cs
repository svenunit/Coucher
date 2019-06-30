using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMusic : MonoBehaviour
{
    public GameObject[] player;
    private void Awake()
    {
        player[0].GetComponent<PlayerInput>().setPlayerNumber(0);
        player[1].GetComponent<PlayerInput>().setPlayerNumber(0);

    }
    private void Start()
    {

        SoundManager.instance.PlayAudioOnSource(SoundManager.instance.bgMusicTitle, SoundManager.instance.audioSourceMain, 1, 0);


    }
}
