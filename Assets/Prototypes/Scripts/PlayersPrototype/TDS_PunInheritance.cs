using Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_PunInheritance : PunBehaviour
{
    #region Fields / Properties
    [SerializeField] private PhotonView photonViewElement = null;
    public int PhotonID
    {
        get { return photonView.viewID; }
    }
    #endregion

    #region Photon Methods
    protected virtual void OnPhotonSerializeView(PhotonStream _stream, PhotonMessageInfo _messageInfo)
    {
        if (_stream.isWriting)
        {

        }
        else
        {

        }
    }
    #endregion
}
