using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private string maptype;

    [SerializeField]
    TextMeshProUGUI OccupancyRateText_ForSchool;
    [SerializeField]
    TextMeshProUGUI OccupancyRateText_ForOutdoor;

    // Start is called before the first frame update
    void Start()
    {
        //This is for scene synchronization: Other players will load the same scene when they join the same room we are in
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region UI Callbacks Methods

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnEnterRoomButtonClicked_Outdoor()
    {
        maptype = MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        { { MultiplayerVRConstants.MAP_TYPE_KEY,maptype} };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterRoomButtonClicked_School()
    {
        maptype = MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        { {MultiplayerVRConstants.MAP_TYPE_KEY, maptype} };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    #endregion

    #region Photon Callback Methods

    //This callback method will be called if the user is failed to join a random room
    //This also give a message which will help in identify the cause
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        
        Debug.Log(message);
        CreateAndJoinRoom();
    }

    //This method is called when we create a room
    public override void OnCreatedRoom()
    {
        Debug.Log("A room is created with the name: "+PhotonNetwork.CurrentRoom.Name);
    }

    //This method is after you get disconnected: In our project, we disconnect when we go "GOHOME" scene from any other scene
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to servers again");
        PhotonNetwork.JoinLobby();

    }

    //This method is called when the player(I) join the room
    public override void OnJoinedRoom()
    {
        Debug.Log("the local player: " + PhotonNetwork.NickName + 
                  " joined to " + PhotonNetwork.CurrentRoom.Name + " player count " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MultiplayerVRConstants.MAP_TYPE_KEY))
        {
            object mapType;
            if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MAP_TYPE_KEY, out mapType))
            {
                Debug.Log("Joined room with the map:" + (string)mapType);
                if ((string)maptype == MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL)
                {
                    //Load school scene
                    PhotonNetwork.LoadLevel("World_School");
                }
                else if((string)maptype == MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR)
                {
                    //load outdoor scene
                    PhotonNetwork.LoadLevel("World_Outdoor");
                }
            }
        }
    }

    //This method is called when other player join the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name +
                                       " Player Count:" + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    //This method is called when some creates a room, joins a room or even someone change a property of a room this method is called
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count == 0)
        {
            OccupancyRateText_ForSchool.text = 0 + "/" + 20;
            OccupancyRateText_ForOutdoor.text = 0 + "/" + 20;
        }

        foreach(RoomInfo room in roomList)
        {
            Debug.Log(room.Name);
            if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR))
            {
                //Update the outdoor map
                Debug.Log("Room is an OUTDOOR map. Player count is: " + room.PlayerCount);
                OccupancyRateText_ForOutdoor.text = room.PlayerCount + "/" + 20;
            }
            else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL))
            {
                //Update the school map
                Debug.Log("Room is a SCHOOL map. Player count is: " + room.PlayerCount);
                OccupancyRateText_ForSchool.text = room.PlayerCount + "/" + 20;
            }
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined to the lobby");
    }

    #endregion

    #region Private Methods

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room_" + maptype + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        string[] roomPropsInLobby = { MultiplayerVRConstants.MAP_TYPE_KEY };

        //There are 2 types of maps: Outdoor and School. 1:Outdoor ="outdoor" , 2:School = "school"
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                        { MultiplayerVRConstants.MAP_TYPE_KEY, maptype } };
        

        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    #endregion


}
