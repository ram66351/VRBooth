using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Demo.PunBasics
{
    public class ChannelTriggerForBooth : MonoBehaviourPunCallbacks
    {
        private BoothManager boothManager;
        private ChatGui ChatUI;
        // Start is called before the first frame update
        void Start()
        {
            boothManager = transform.parent.gameObject.GetComponent<BoothManager>();
            ChatUI = (ChatGui)GameObject.FindObjectOfType(typeof(ChatGui));
            GetComponent<MeshRenderer>().enabled = false;
        }

        void OnTriggerEnter(Collider coll)
        {
            //Debug.Log("OnTigger Enter : " + boothManager.ChannelID);
            if (coll.gameObject.tag == "Player")
            {
                if(coll.gameObject.GetComponent< PlayerManager>().isMine)
                {
                    ChatUI.EnableDisableChannel(boothManager.ChannelID);
                    coll.gameObject.GetComponent<PlayerManager>().TotalTriggers++;
                }
            }
        }

        void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.tag == "Player")
            {
                if (coll.gameObject.GetComponent<PlayerManager>().isMine)
                {
                    
                    coll.gameObject.GetComponent<PlayerManager>().TotalTriggers--;
                    if(coll.gameObject.GetComponent<PlayerManager>().TotalTriggers == 0)
                    {
                        ChatUI.EnablePublicChannel();
                    }
                }
            }
        }
    }
}
