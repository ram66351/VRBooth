using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class VideoButton : MonoBehaviourPunCallbacks
{
    public NetworkVideoPlayer networkVidPlayer;
    public enum ButtonType
    {
        play_Pause, stop 
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        //if (!photonView.IsMine)
        //  return;
        Debug.Log("on mouse down");
        networkVidPlayer.PlayOrPause();
    }
}
