using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.Native;
using System.Collections.Generic;
using System.Linq;

namespace FrostweepGames.UniversalMicrophoneLibrary.Examples
{
    [RequireComponent(typeof(AudioSource))]
    public class Example : MonoBehaviour
    {
        private AudioClip _workingClip;

        public Text permissionStatusText;

        public AudioSource audioSource;

        public Button startRecordButton,
                      stopRecordButton,
                      playRecordedAudioButton,
                      requestPermissionButton;

        public List<AudioClip> recordedClips;

        public int frequency = 44100;

        public int recordingTime = 2;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            startRecordButton.onClick.AddListener(StartRecord);
            stopRecordButton.onClick.AddListener(StopRecord);
            playRecordedAudioButton.onClick.AddListener(PlayRecordedAudio);
            requestPermissionButton.onClick.AddListener(RequestPermission);
        }

		private void Update()
		{
            permissionStatusText.text = string.Format("Microphone permission: {0}", 
                CustomMicrophone.HasMicrophonePermission() ? "<color=green>granted</color>" : "<color=red>denined</color>");
        }

		private void RequestPermission()
        {
            CustomMicrophone.RequestMicrophonePermission();
        }

        private void StartRecord()
        {
            if (!CustomMicrophone.HasConnectedMicrophoneDevices())
                return;

            _workingClip = CustomMicrophone.Start(CustomMicrophone.devices[0], false, recordingTime, frequency);
        }

        private void StopRecord()
        {
            if (!CustomMicrophone.HasConnectedMicrophoneDevices())
                return;

            if (!CustomMicrophone.IsRecording(CustomMicrophone.devices[0]))
                return;

            CustomMicrophone.End(CustomMicrophone.devices[0]); 

            recordedClips.Add(MakeCopy($"copy{recordedClips.Count}", recordingTime, frequency, _workingClip));

            audioSource.clip = recordedClips.Last();
            audioSource.Play();
        }

        private void PlayRecordedAudio()
        {
            if (_workingClip == null)
                return;

            audioSource.clip = _workingClip;
            audioSource.Play();
        }

        private AudioClip MakeCopy(string name, int recordingTime, int frequency, AudioClip sourceClip)
		{
            float[] array = new float[recordingTime * frequency];
            if (CustomMicrophone.GetRawData(ref array, sourceClip))
            {
                AudioClip audioClip = AudioClip.Create(name, recordingTime * frequency, 1, frequency, false);
                audioClip.SetData(array, 0);
                audioClip.LoadAudioData();

                return audioClip;
            }

            return null;

        }
    }
}