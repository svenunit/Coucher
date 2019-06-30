using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour
{
    public GameObject[] player;

    public SpriteRenderer[] playerSprites;

    public bool player1Assigned;
    public bool player2Assigned;

    public Image player1ControllerSign;
    public Image player2ControllerSign;



    private void Update()
    {

        if (player[0].GetComponent<PlayerInput>().getPlayerNumber() == 0)
        {

            playerSprites[0].color = new Color32(255, 255, 255, 10);
        }
        else
        {
            player1ControllerSign.color = new Color32(255, 255, 255, 255);
            playerSprites[0].color = new Color32(255, 255, 255, 255);
        }


        if (player[1].GetComponent<PlayerInput>().getPlayerNumber() == 0)
        {
            playerSprites[1].color = new Color32(255, 255, 255, 10);
        }
        else
        {
            player2ControllerSign.color = new Color32(255, 255, 255, 255);
            playerSprites[1].color = new Color32(255, 255, 255, 255);
        }
        ////////

        /*
        if (Input.GetAxis("RTriggerP1") >= .9f && !player1Assigned)
        {
            if (player[0].GetComponent<PlayerInput>().getPlayerNumber() == 0)
            {
                player[0].GetComponent<PlayerInput>().setPlayerNumber(1);
                SoundManager.instance.PlayAudioOnSource(SoundManager.instance.player1Connect, SoundManager.instance.audioSourceSFXUI, 0, 0);

                player1Assigned = true;

            }
            else
            {
                player[1].GetComponent<PlayerInput>().setPlayerNumber(1);
                SoundManager.instance.PlayAudioOnSource(SoundManager.instance.player1Connect, SoundManager.instance.audioSourceSFXUI, 0, 0);

                player1Assigned = true;

            }

        }
        //////////
        if (Input.GetAxis("RTriggerP2") >= .9f && !player2Assigned)
        {
            if (player[0].GetComponent<PlayerInput>().getPlayerNumber() == 0)
            {
                player[0].GetComponent<PlayerInput>().setPlayerNumber(2);
                SoundManager.instance.PlayAudioOnSource(SoundManager.instance.player2Connect, SoundManager.instance.audioSourceSFXUI, 0, 0);

                player2Assigned = true;

            }
            else if (player[1] != null)
            {

                player[1].GetComponent<PlayerInput>().setPlayerNumber(2);
                SoundManager.instance.PlayAudioOnSource(SoundManager.instance.player2Connect, SoundManager.instance.audioSourceSFXUI, 0, 0);

                player2Assigned = true;


            }
        }
        */

        ////////


        if (Input.GetAxis("RTriggerP1") >= .9f && !player1Assigned)
        {
            if (player[0].GetComponent<PlayerInput>().getPlayerNumber() == 0)
            {
                player[0].GetComponent<PlayerInput>().setPlayerNumber(1);
                SoundManager.instance.PlayAudioOnSource(SoundManager.instance.player1Connect, SoundManager.instance.audioSourceSFXUI, 0, 0);

                player1Assigned = true;

            }
          
            

        }
        //////////
        if (Input.GetAxis("RTriggerP2") >= .9f && !player2Assigned)
        {
            if (player[1].GetComponent<PlayerInput>().getPlayerNumber() == 0)
            {
                player[1].GetComponent<PlayerInput>().setPlayerNumber(2);
                SoundManager.instance.PlayAudioOnSource(SoundManager.instance.player2Connect, SoundManager.instance.audioSourceSFXUI, 0, 0);

                player2Assigned = true;

            }
         
        }
    }
}
