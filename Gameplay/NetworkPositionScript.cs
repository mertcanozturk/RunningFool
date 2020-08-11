using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkPositionScript : MonoBehaviourPun, IPunObservable
{
    Vector3 networkPosition;
    Quaternion networkRotation;

    public float rotationSmoothness = 50;

    float lag;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
    void Update()
    {
        if (GameManager.instance.IsAIMode) return;

        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 15);

            if (Vector3.Distance(transform.rotation.eulerAngles, networkRotation.eulerAngles) < 2)
                if (networkRotation.z != 0)
                    transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * rotationSmoothness);
                else
                    transform.rotation = networkRotation;
            else
                transform.rotation = networkRotation;

        }
    }

}
