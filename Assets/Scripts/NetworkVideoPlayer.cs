using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Photon.Pun;

public class NetworkVideoPlayer : MonoBehaviourPunCallbacks,IPunObservable
{
    public VideoPlayer vPlayer;
    // Start is called before the first frame update
    public bool Toggle;
    public bool isPlay = false;
    public GameObject PlayPauseButton;
    public Texture2D playIco;
    public Texture2D pauseIco;

    private AudioSource audioSource;
    private Material PlayButtonMat;
    private PhotonView PV;

    public bool isInitialized;
    private double playerTime;
    public string videoStatus;

    public string videoID;

    void Start()
    {
        Init(vPlayer.url);
    }

    public void Init(string url)
    { 
        ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable
        {
            { videoID, "stop" }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);


        PV = GetComponent<PhotonView>();
        vPlayer.url = url; 
            audioSource = GetComponent<AudioSource>();
            vPlayer.SetTargetAudioSource(0, audioSource);
            vPlayer.EnableAudioTrack(0, true);
            vPlayer.controlledAudioTrackCount = 1;
            PlayButtonMat = PlayPauseButton.GetComponent<Renderer>().material;
            isInitialized = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isInitialized)
            return;

        if (isPlay)
        {
            vPlayer.Play();

            PlayButtonMat.mainTexture = pauseIco;
        }
        else
        {
            vPlayer.Pause();
            PlayButtonMat.mainTexture = playIco;
        }

        playerTime = vPlayer.time;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(videoID, out object status))
        {
            var _status = (string)status; // This is the Room properties' value
            if (_status == "play")
                isPlay = true;
            else
                isPlay = false;
        }
    }

    public void PlayOrPause()
    {
        isPlay = !isPlay;

        if(isPlay)
        {
            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable
            {
                { videoID, "play" }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        }
        else
        {
            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable
            {
                { videoID, "pause" }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        }
    }

    public void Stop()
    {
        if (vPlayer.isPlaying || vPlayer.isPaused)
        {
            vPlayer.Stop();
            isPlay = false;
        }
    }

    #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!isInitialized)
            return;

        Debug.Log("OnPhotonSerializeView Called");Debug.Log("OnPhotonSerializeView Called");
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            //stream.SendNext(isPlay);
            //stream.SendNext(playerTime);
        }
        else
        {
            // Network player, receive data
            //this.isPlay = (bool)stream.ReceiveNext();
            //this.playerTime = (double)stream.ReceiveNext();
        }
    }

    public  void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("OnRoomPropertiesUpdate");
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(videoID, out object status))
        {
            var arr = (string)status; // This is the Room properties' value
            if (status == "play")
                isPlay = true;
            else
                isPlay = false;
        }
    }
    #endregion
}

