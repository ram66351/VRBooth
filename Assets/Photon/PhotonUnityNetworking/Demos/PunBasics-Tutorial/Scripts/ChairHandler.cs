
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;

    public class ChairHandler : MonoBehaviour
{
    public Transform playerReferencePos;
    public Transform forwardRef;


    void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
        GameManager.Instance.LocalPlayer.GetComponent<PlayerManager>().ShowSitButton(this);
    }

    public void MakeThePlayerToSit(Transform player)
    {
        player.position = playerReferencePos.position;
        Vector3 lookPos = new Vector3(forwardRef.position.x, player.position.y, forwardRef.position.z);
        player.LookAt(lookPos);
    }


}
