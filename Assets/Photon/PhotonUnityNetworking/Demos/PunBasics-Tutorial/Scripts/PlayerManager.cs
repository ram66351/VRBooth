// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player instance
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Photon.Pun.Demo.PunBasics
{
	#pragma warning disable 649

    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        public GameObject miniSphere;
        public Material greenMat;
        public GameObject minimapCamera;
        private ChairHandler chairHandler; 
        #endregion

        #region Private Fields

        public static float SoundRange = 10.0f;

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        private GameObject playerUiPrefab;

        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;
        private VideoPlayer[] videoPlayers;

        //True, when the user is firing
        bool IsFiring;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        /// 

        public bool isMine;
        public int TotalTriggers;

        private GameObject[] StallNames;
        private Animator animator;
        private Button clapButton;
        private Button SitButton;
        private Button StandButton;

        public float StallVisibilityRange = 50;
        public float ClapTime = 3;
        public bool isClapping = true;

        public void Awake()
        {
            if (this.beams == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> Beams Reference.", this);
            }
            else
            {
                this.beams.SetActive(false);
            }

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                isMine = true;
                LocalPlayerInstance = gameObject;
                videoPlayers = (VideoPlayer[])GameObject.FindObjectsOfType(typeof(VideoPlayer));
            }

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        public void Start()
        {
            CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();
            animator = GetComponent<Animator>();
            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }

            // Create the UI
            if (this.playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(this.playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

            if (photonView.IsMine)
            {
                miniSphere.GetComponent<Renderer>().material = greenMat;
                GameManager.Instance.LocalPlayer = gameObject;

                clapButton = GameObject.Find("Button_Clap").GetComponent<Button>();
                clapButton.onClick.AddListener(Clap);

                Button resetAnim = GameObject.Find("ResetAnim").GetComponent<Button>();
                resetAnim.onClick.AddListener(ResetExpressionAnimations);

                Button gangnamDance = GameObject.Find("Gangnam").GetComponent<Button>();
                gangnamDance.onClick.AddListener(GangnamStyle);

                Button sambaDance = GameObject.Find("Samba").GetComponent<Button>();
                sambaDance.onClick.AddListener(SambaStyle);

                SitButton = GameObject.Find("Sit").GetComponent<Button>();
                SitButton.onClick.AddListener(SitAnim);

                StandButton = GameObject.Find("Stand").GetComponent<Button>();
                StandButton.onClick.AddListener(StandUp);

                SitButton.gameObject.SetActive(false);
                StandButton.gameObject.SetActive(false);
            }
            else
            {
                minimapCamera.SetActive(false);
            }

            StallNames = GameObject.FindGameObjectsWithTag("StallName");

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }


        void Update()
        {
            if (photonView.IsMine)
            {
                foreach(VideoPlayer vplayer in videoPlayers)
                {
                    AudioSource audioSource = vplayer.GetComponent<AudioSource>();

                    float distance = Vector3.Distance(gameObject.transform.position, audioSource.gameObject.transform.position);

                    if (distance < SoundRange)
                    {
                       // audioSource.volume = 1;
                        //Debug.Log("Distance less than 10");

                        if (distance < 2)
                            distance = 0;

                        vplayer.SetDirectAudioVolume(0, 1.0f - (distance/SoundRange));
                    }
                        
                    else
                    {
                        audioSource.volume = 0;
                        vplayer.SetDirectAudioVolume(0, 0);
                    }
                }

                for(int i=0; i< StallNames.Length; i++)
                {
                    float dist = Vector3.Distance(transform.position, StallNames[i].transform.position);

                    if(dist < StallVisibilityRange)
                    {
                        StallNames[i].SetActive(true);
                        Vector3 playerPos = transform.position;
                        Vector3 lookPos = new Vector3(playerPos.x, -StallNames[i].transform.position.y, playerPos.z);
                        StallNames[i].transform.LookAt(lookPos);
                    }
                    else
                    {
                        StallNames[i].SetActive(false);
                    }
                   
                }

            }
        }

		public override void OnDisable()
		{
			// Always call the base to remove callbacks
			base.OnDisable ();

			#if UNITY_5_4_OR_NEWER
			UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
			#endif
		}


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// Process Inputs if local player.
        /// Show and hide the beams
        /// Watch for end of game, when local player health is 0.
        /// </summary>
        /*public void Update()
        {
            // we only process Inputs and check health if we are the local player
            if (photonView.IsMine)
            {
                this.ProcessInputs();

                if (this.Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }
            }

            if (this.beams != null && this.IsFiring != this.beams.activeInHierarchy)
            {
                this.beams.SetActive(this.IsFiring);
            }
        }*/

        /// <summary>
        /// MonoBehaviour method called when the Collider 'other' enters the trigger.
        /// Affect Health of the Player if the collider is a beam
        /// Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
        /// One could move the collider further away to prevent this or check if the beam belongs to the player.
        /// </summary>
        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }


            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }
            
            this.Health -= 0.1f;
        }

        /// <summary>
        /// MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
        /// We're going to affect health while the beams are interesting the player
        /// </summary>
        /// <param name="other">Other.</param>
        public void OnTriggerStay(Collider other)
        {
            // we dont' do anything if we are not the local player.
            if (!photonView.IsMine)
            {
                return;
            }

            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }

            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
            this.Health -= 0.1f*Time.deltaTime;
        }

       

        #if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
        #endif


        /// <summary>
        /// MonoBehaviour method called after a new level of index 'level' was loaded.
        /// We recreate the Player UI because it was destroy when we switched level.
        /// Also reposition the player if outside the current arena.
        /// </summary>
        /// <param name="level">Level index loaded</param>
        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        public void Clap()
        {
            animator.SetBool("Clap", true);
            animator.SetBool("Gangnam", false);
            animator.SetBool("Samba", false);
            StartCoroutine(StopClap("Clap", 3));
        }

        public void GangnamStyle()
        {
            
            animator.SetBool("Clap", false);
            animator.SetBool("Gangnam", true);
            animator.SetBool("Samba", false);
            StartCoroutine(StopClap("Gangnam", 12));
        }

        public void SambaStyle()
        {
            animator.SetBool("Clap", false);
            animator.SetBool("Gangnam", false);
            animator.SetBool("Samba", true);
            StartCoroutine(StopClap("Samba", ClapTime));
        }

        public void ResetExpressionAnimations()
        {
            animator.SetBool("Clap", false);
            animator.SetBool("Gangnam", false);
            animator.SetBool("Samba", false);
        }

        public void SitAnim()
        {
            ResetExpressionAnimations();
            SitButton.gameObject.SetActive(false);
            StandButton.gameObject.SetActive(true);
            animator.SetBool("Sit", true);
            chairHandler.MakeThePlayerToSit(transform);
            chairHandler.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }

        public void StandUp()
        {
            ResetExpressionAnimations();
            StandButton.gameObject.SetActive(false);
            animator.SetBool("Sit", false);
            chairHandler.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }

        IEnumerator StopClap(string animName, float duration)
        {
            yield return new WaitForSeconds(duration);
            animator.SetBool(animName, false);
        }

        public void ShowSitButton(ChairHandler chair)
        {
            chairHandler = chair;
            SitButton.gameObject.SetActive(true);
            //chairHandler = chair;
        }

        #endregion

        #region Private Methods


#if UNITY_5_4_OR_NEWER
		void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
		{
			this.CalledOnLevelWasLoaded(scene.buildIndex);
		}
#endif

        /// <summary>
        /// Processes the inputs. This MUST ONLY BE USED when the player has authority over this Networked GameObject (photonView.isMine == true)
        /// </summary>
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // we don't want to fire when we interact with UI buttons for example. IsPointerOverGameObject really means IsPointerOver*UI*GameObject
                // notice we don't use on on GetbuttonUp() few lines down, because one can mouse down, move over a UI element and release, which would lead to not lower the isFiring Flag.
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    //	return;
                }

                if (!this.IsFiring)
                {
                    this.IsFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (this.IsFiring)
                {
                    this.IsFiring = false;
                }
            }
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this.IsFiring);
                stream.SendNext(this.Health);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }

        #endregion
    }
}