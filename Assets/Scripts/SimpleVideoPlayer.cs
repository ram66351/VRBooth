using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class SimpleVideoPlayer : MonoBehaviour
{
    public VideoPlayer vPlayer;
    // Start is called before the first frame update
    public bool Toggle;
    public bool isPlay = true;
    public Image PlayPauseButton;
    public Sprite playIco;
    public Sprite pauseIco;

    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        vPlayer.SetTargetAudioSource(0, audioSource);
        vPlayer.EnableAudioTrack(0, true);
        vPlayer.controlledAudioTrackCount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayOrPause()
    {
        isPlay = !isPlay;
        if (isPlay)
        {
            vPlayer.Play();
            PlayPauseButton.sprite = pauseIco;
        }
        else
        {
            vPlayer.Pause();
            PlayPauseButton.sprite = playIco;
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
}
