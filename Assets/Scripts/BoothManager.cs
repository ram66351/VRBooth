using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;

public class BoothManager : MonoBehaviour
{
    public Booth boothInfo;

    public TextMesh boothName;
    public GameObject[] banners = new GameObject[3];
    public GameObject videoScreen;
    public Material BannerMat;
    public string ChannelID;
    public TextMesh StallNameIndicator;
    public Transform SpawnPoint;
    public Transform Lobby;
    public Button telePortationButtonPrefab;
    public Button teleportToLobbyButton;
     
    public enum Wing
    {
        leftWing, rightWing
    }

    public Wing wing;

    public Transform SignBoardParent;
    

    // Start is called before the first frame update
    void Start()
    {
        //boothName.text = boothInfo.Name;
        gameObject.name = ChannelID;

        if(banners.Length == boothInfo.bannerUrls.Length)
        {
            for(int i=0; i<banners.Length; i++)
            {
                StartCoroutine(DownloadImage(boothInfo.bannerUrls[i], banners[i]));
            }
        }

        videoScreen.GetComponent<NetworkVideoPlayer>().Init(boothInfo.videoUrl);

        StallNameIndicator.text = ChannelID;

        GameObject teleButton = Instantiate(telePortationButtonPrefab.gameObject) as GameObject;
        if (wing == Wing.leftWing)
            SignBoardParent = GameObject.Find("LeftWing_SignBoard").transform;
        else
            SignBoardParent = GameObject.Find("RightWing_SignBoard").transform;

        Lobby = GameObject.Find("Lobby").transform;

        teleButton.gameObject.transform.SetParent(SignBoardParent, false);
        teleButton.GetComponent<Button>().onClick.AddListener(TeleportMeHere);
        teleButton.GetComponentInChildren<Text>().text = ChannelID.Split('.')[1].ToString();

        teleportToLobbyButton.GetComponent<Button>().onClick.AddListener(TeleportToLobby);
    }


    IEnumerator DownloadImage(string MediaUrl, GameObject bannerGObj)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        request.SetRequestHeader("Access-Control-Allow-Origin", "*");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            //bannerGObj.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Texture texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Material mat = Instantiate(BannerMat);
            mat.mainTexture = texture;
            bannerGObj.GetComponent<Renderer>().material = mat;
        }      
    }

    public void TeleportMeHere()
    {
        
        if(GameManager.Instance.LocalPlayer != null)
        {
            //Debug.Log("Teleported to " + ChannelID);

            Vector3 playerPos = GameManager.Instance.LocalPlayer.transform.position;
            float ranX = Random.Range(-5.0f, 5.0f);
            float ranY = Random.Range(-5.0f, 5.0f);

            Vector3 NewPos = new Vector3(SpawnPoint.position.x + ranX, playerPos.y, SpawnPoint.position.z + ranY);
            Vector3 lookPos = new Vector3(transform.position.x, playerPos.y, transform.position.z);

            GameManager.Instance.LocalPlayer.transform.position = NewPos;
            GameManager.Instance.LocalPlayer.transform.LookAt(lookPos); 
        }
       
    }

    public void TeleportToLobby()
    {
        if(Lobby != null)
        {
            Vector3 playerPos = GameManager.Instance.LocalPlayer.transform.position;
            float ranX = Random.Range(-5.0f, 5.0f);
            float ranY = Random.Range(-5.0f, 5.0f);

            Vector3 NewPos = new Vector3(Lobby.position.x + ranX, playerPos.y, Lobby.position.z + ranY);

            GameObject LobbyVideo = GameObject.Find("LobbyVideo");
            Vector3 lookPos = new Vector3(LobbyVideo.transform.position.x, playerPos.y, LobbyVideo.transform.position.z);

            GameManager.Instance.LocalPlayer.transform.position = NewPos;
            GameManager.Instance.LocalPlayer.transform.LookAt(lookPos);
        }
    }
}
