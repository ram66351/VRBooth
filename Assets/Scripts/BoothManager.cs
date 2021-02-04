using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BoothManager : MonoBehaviour
{
    public Booth boothInfo;

    public TextMesh boothName;
    public GameObject[] banners = new GameObject[3];
    public GameObject videoScreen;
    public Material BannerMat;

    // Start is called before the first frame update
    void Start()
    {
        boothName.text = boothInfo.Name;

        if(banners.Length == boothInfo.bannerUrls.Length)
        {
            for(int i=0; i<banners.Length; i++)
            {
                StartCoroutine(DownloadImage(boothInfo.bannerUrls[i], banners[i]));
            }
        }

        videoScreen.GetComponent<NetworkVideoPlayer>().Init(boothInfo.videoUrl);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DownloadImage(string MediaUrl, GameObject bannerGObj)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
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
}
