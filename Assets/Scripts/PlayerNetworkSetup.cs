using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject LocalXRRigGameobject;

    public GameObject AvatarHeadGameobject;
    public GameObject AvatarBodyGameobject;

    public GameObject MainAvatarGameobject;

    public GameObject[] AvatarModelPrefabs;

    public TextMeshProUGUI PlayerName_Text;
    
    /// <summary>
    /// This will keep XRRig of each player active
    /// </summary>
    void Start()
    {
        //Setup the player
        //This method tells whether the instantiated player is US or not
        if (photonView.IsMine)
        {
            //true: player is local
            LocalXRRigGameobject.SetActive(true);
            gameObject.GetComponent<MovementController>().enabled = true;
            gameObject.GetComponent<AvatarInputConverter>().enabled = true;
            SetLayerRecursively(AvatarHeadGameobject, 11);
            SetLayerRecursively(AvatarBodyGameobject, 12);

            //Getting Avatar selection data so that the correct avatar models can be instantiated.
            object avatarSelectionNumber;
            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER,out avatarSelectionNumber))
            {
                Debug.Log("Avatar Selection number: " + (int)avatarSelectionNumber);
            }


            TeleportationArea[] teleportationAreas = GameObject.FindObjectsOfType<TeleportationArea>();
            if (teleportationAreas.Length > 0)
            {
                Debug.Log("Found " + teleportationAreas.Length + " teleportation area");
                foreach(TeleportationArea area in teleportationAreas)
                {
                    area.teleportationProvider = LocalXRRigGameobject.GetComponent<TeleportationProvider>();
                    photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, (int)avatarSelectionNumber);
                }
            }

            MainAvatarGameobject.AddComponent<AudioListener>();

        }
        else
        {
            //false: player is remote
            LocalXRRigGameobject.SetActive(false);
            gameObject.GetComponent<MovementController>().enabled = false;
            gameObject.GetComponent<MovementController>().enabled = false;

            //Remote player can be seen by the local player
            //So, we set the avatar head and body  to default layer
            SetLayerRecursively(AvatarHeadGameobject, 0);
            SetLayerRecursively(AvatarBodyGameobject, 0);
        }

        if (PlayerName_Text != null)
        {
            PlayerName_Text.text = photonView.Owner.NickName;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// We do not render head and body as they block the view but we have to render it for remote player.
    /// This method helps us set the layer of the gameobjects
    /// </summary>
    /// <param name="go">Gameobject of which layer number we want</param>
    /// <param name="layerNumber">layer number of the mentioned gameobject in the layer list</param>
    void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach(Transform trans in go.GetComponentInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    [PunRPC]
    public void InitializeSelectedAvatarModel(int avatarSelectionNumber)
    {
        GameObject selectedAvatarGameobject = Instantiate(AvatarModelPrefabs[avatarSelectionNumber], LocalXRRigGameobject.transform);

        AvatarInputConverter avatarInputConverter = transform.GetComponent<AvatarInputConverter>();
        AvatarHolder avatarHolder = selectedAvatarGameobject.GetComponent<AvatarHolder>();
        SetUpAvatarGameobject(avatarHolder.HeadTransform, avatarInputConverter.AvatarHead);
        SetUpAvatarGameobject(avatarHolder.BodyTransform, avatarInputConverter.AvatarBody);
        SetUpAvatarGameobject(avatarHolder.HandLeftTransform, avatarInputConverter.AvatarHand_Left);
        SetUpAvatarGameobject(avatarHolder.HandRightTransform, avatarInputConverter.AvatarHand_Right);
    }

    void SetUpAvatarGameobject(Transform avatarModelTransform, Transform mainAvatarTransform)
    {
        avatarModelTransform.SetParent(mainAvatarTransform);
        avatarModelTransform.localPosition = Vector3.zero;
        avatarModelTransform.localRotation = Quaternion.identity;
    }


}
