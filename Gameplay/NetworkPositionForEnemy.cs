using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkPositionForEnemy : MonoBehaviourPun, IPunObservable
{
    Quaternion networkRotation = Quaternion.identity;

    public float rotationSmoothness = 50;

    float lag;

    bool firstTimePlaced = false;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
    void Update()
    {
        if (!firstTimePlaced && networkRotation != Quaternion.identity)
        {
            if (!photonView.IsMine)
                if (Vector3.Distance(transform.rotation.eulerAngles, networkRotation.eulerAngles) < 2)
                    if (networkRotation != new Quaternion(0, 0, 0, 0))
                        transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * rotationSmoothness);
                    else
                        transform.rotation = networkRotation;
                else
                    transform.rotation = networkRotation;
            firstTimePlaced = true;

        }

    }
}
