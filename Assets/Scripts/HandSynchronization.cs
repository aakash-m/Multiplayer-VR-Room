using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HandSynchronization : MonoBehaviour, IPunObservable
{
    public Transform leftHandTransform;

    private PhotonView m_PhotonView;

    //Left Hand Sync
    private float m_Distance_LeftHand;

    //Position
    private Vector3 m_Direction_LeftHand;
    private Vector3 m_NetworkPosition_LeftHand;
    private Vector3 m_StoredPosition_LeftHand;

    //Rotation
    private Quaternion m_NetworkRotation_LeftHand;
    private float m_Angle_LeftHand;

    bool m_FirstTake = false;

    private void OnEnable()
    {
        m_FirstTake = true;
    }


    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

        //Left Hand Synch Init
        m_StoredPosition_LeftHand = leftHandTransform.localPosition;
        m_NetworkPosition_LeftHand = Vector3.zero;
        m_NetworkRotation_LeftHand = Quaternion.identity;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check if photon is not mine then update because it is for remote player: others
        if (!m_PhotonView.IsMine)
        {

            leftHandTransform.localPosition = Vector3.MoveTowards(leftHandTransform.localPosition,
                                                                m_NetworkPosition_LeftHand,
                                                                m_Distance_LeftHand * (1.0f / PhotonNetwork.SerializationRate));
            leftHandTransform.localRotation = Quaternion.RotateTowards(leftHandTransform.localRotation,
                                                                     m_NetworkRotation_LeftHand,
                                                                     m_Angle_LeftHand * (1.0f / PhotonNetwork.SerializationRate));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Local player: me
            //Send left hand position data
            m_Direction_LeftHand = leftHandTransform.localPosition - m_StoredPosition_LeftHand;
            m_StoredPosition_LeftHand = leftHandTransform.localPosition;

            stream.SendNext(leftHandTransform.localPosition);
            stream.SendNext(m_Direction_LeftHand);

            //Send left hand rotation data
            stream.SendNext(leftHandTransform.localRotation);

        }
        else
        {
            //Remote player: others
            //Get left hand position data
            m_NetworkPosition_LeftHand = (Vector3)stream.ReceiveNext();
            m_Direction_LeftHand = (Vector3)stream.ReceiveNext();

            if (m_FirstTake)
            {
                leftHandTransform.localPosition = m_NetworkPosition_LeftHand;
                m_Distance_LeftHand = 0;
            }
            else
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                m_NetworkPosition_LeftHand += m_Direction_LeftHand * lag;
                m_Distance_LeftHand = Vector3.Distance(leftHandTransform.localPosition, m_NetworkPosition_LeftHand);
            }

            //Get left hand rotation data
            m_NetworkRotation_LeftHand = (Quaternion)stream.ReceiveNext();
            if (m_FirstTake)
            {
                m_Angle_LeftHand = 0;
                leftHandTransform.localRotation = m_NetworkRotation_LeftHand;
            }
            else
            {
                m_Angle_LeftHand = Quaternion.Angle(leftHandTransform.localRotation, m_NetworkRotation_LeftHand);
            }

            if (m_FirstTake)
            {
                m_FirstTake = false;
            }

        }
    }

    
}
