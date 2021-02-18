using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

/// <summary>
/// With inherited class "MonoBehaviourPunCallbacks" we will be able to access photon callbacks 
/// </summary>
public class LoginManager : MonoBehaviourPunCallbacks
{

    public TMP_InputField playerName_InputField;

    #region UNITY Methods

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Photon CallbackMethods

    //This method is called when internet connection is established
    public override void OnConnected()
    {
        //base.OnConnected();
        Debug.Log("OnConnected. Server is available");
    }

    //This method is called when user is successfully connected to server 
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server with player name: " + PhotonNetwork.NickName);
        PhotonNetwork.LoadLevel("HomeScene");
    }

    #endregion

    #region UI Callback Methods

    public void ConnectToPhotonServer()
    {
        if (playerName_InputField != null)
        {
            //Set the player in the virtual room
            PhotonNetwork.NickName = playerName_InputField.text;
            //For connecting with photon
            PhotonNetwork.ConnectUsingSettings();
        }

        
    }

    #endregion


}
