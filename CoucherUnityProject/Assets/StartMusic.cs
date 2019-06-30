using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMusic : MonoBehaviour
{
    private void Start()
    {
        SoundManager.instance.PlayAudioOnSource(SoundManager.instance.bgMusicTitle, SoundManager.instance.audioSourceMain, 1, 0);
    }
}
