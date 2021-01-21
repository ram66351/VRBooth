using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Photon.Pun;
public class NetworkVideoPlayer : MonoBehaviourPunCallbacks, IPunObservable
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

    void Start()
    {
        PV = GetComponent<PhotonView>();

            audioSource = GetComponent<AudioSource>();
            vPlayer.SetTargetAudioSource(0, audioSource);
            vPlayer.EnableAudioTrack(0, true);
            vPlayer.controlledAudioTrackCount = 1;
            PlayButtonMat = PlayPauseButton.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void LateUpdate()
    {
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
    }

    public void PlayOrPause()
    {
        isPlay = !isPlay;
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
        Debug.Log("OnPhotonSerializeView Called");Debug.Log("OnPhotonSerializeView Called");
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(isPlay);
        }
        else
        {
            // Network player, receive data
            this.isPlay = (bool)stream.ReceiveNext();
        }
    }

    #endregion
}

