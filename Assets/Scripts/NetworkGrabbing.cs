using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkGrabbing : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{

    PhotonView m_photonView;
    private Rigidbody rb;
    public bool isBeingHeld; //To keep track if object is being held or not

    private void Awake()
    {
        m_photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isBeingHeld)
        {
            rb.isKinematic = true;
            gameObject.layer = 13; //Change layer to InHand for other players unable to grab. Layer is changed because if the layer is interactable then other player can grab object from your hand even if you held it.
        }
        else
        {
            rb.isKinematic = false;
            gameObject.layer = 8; //Change layer back to Interactable for other players to grab
        }
        
    }

    private void TransferOwenrship()
    {
        m_photonView.RequestOwnership();
    }

    public void OnSelectEnter()
    {
        Debug.Log("Grabbed");

        //This is for making RPC call to all the players: AllBuffered- sends everyone in the room
        m_photonView.RPC("StartNetworkGrabbing", RpcTarget.AllBuffered);

        if (m_photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            Debug.Log("We do not request for ownership. It's already mine");
        }
        else
        {
            TransferOwenrship();
        }
        
    }

    public void OnSelectExit()
    {
        Debug.Log("Released");
        m_photonView.RPC("StopNetworkGrabbing", RpcTarget.AllBuffered);
    }

    //Called when ownership transfer requested for an object
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        //This condition checks if the targetView (photonView) of the grabbed object is not the equal to the m_photonView of the this object
        if (targetView != m_photonView)
        {
            return;
        }
        Debug.Log("OnOwnership requested for: " + targetView.name + " from " + requestingPlayer);
        m_photonView.TransferOwnership(requestingPlayer);
    }

    //Called when ownership transfer request is completed
    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log("Ownership transfer is complete. New owner: " + targetView.Owner.NickName);
    }


    [PunRPC]    //This is to make the following method PUN RPC method
    public void StartNetworkGrabbing()
    {
        isBeingHeld = true;
    }

    [PunRPC]
    public void StopNetworkGrabbing()
    {
        isBeingHeld = false;
    }

}
