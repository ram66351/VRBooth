using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.WebGLPUNVoice;

public class SpeechHandler : MonoBehaviour
{
    private Recorder recorder;
    public Button BtnSpeak;
    public Button BtnStopSpeak;

    // Start is called before the first frame update
    void Start()
    {
        recorder = GetComponent<Recorder>();
        recorder.recording = false;
        BtnSpeak.gameObject.SetActive(true);
    }

    public void EnableSpeech()
    {
        BtnSpeak.gameObject.SetActive(false);
        BtnStopSpeak.gameObject.SetActive(true);
        //recorder.recording = true;
        recorder.StartRecord();
    }

    public void DisableSpeech()
    {
        BtnSpeak.gameObject.SetActive(true);
        BtnStopSpeak.gameObject.SetActive(false);
        //recorder.recording = false;
        recorder.StopRecord();
    }
}
