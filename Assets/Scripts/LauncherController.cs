using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Hive
{
    public class LauncherController : MonoBehaviourPunCallbacks
    {
        string gameVersion = "1";

        [SerializeField]
        private byte maxPlayersPerRoom = 2;


        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;    
        }

        void Start()
        {
            Connect();    
        }

        public void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster was called by PUN");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("OnDisconnected was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed was called by PUN");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom was called by PUN");
        }
    }
}


