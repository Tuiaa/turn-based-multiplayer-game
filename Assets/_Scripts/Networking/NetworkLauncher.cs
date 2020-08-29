using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    /* GENERAL */
    [SerializeField] private string gameVersion = "1";
    private byte maxPlayersPerRoom = 2;

    /* Names */
    private InputField playerNameField;
    private string playerName = "";
    private InputField roomNameField;
    private string roomName = "";

    /* Status */
    private bool isConnecting = false;
    public Text playerStatus;
    public Text connectionStatus;

    /* UI */
    private GameObject controlPanel;
    public GameObject roomJoinUI;
    public GameObject buttonLoadArena;
    public GameObject buttonJoinRoom;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log(GameConstants.LOG_CONNECTING_TO_PHOTON);

        roomJoinUI.SetActive(false);
        buttonLoadArena.SetActive(false);

        ConnectToPhoton();
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    void ConnectToPhoton()
    {
        connectionStatus.text = GameConstants.STATUS_CONNECTING;
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            Debug.Log("PhotonNetwork.IsConnected | Trying to create/Join Room " + roomNameField.text);

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayersPerRoom;
            TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default);

            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
        }
    }
    public void LoadArena()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PhotonNetwork.LoadLevel("MainArena");
        }
        else
        {
            playerStatus.text = "Minimum 2 players required to Load Arena!";
        }
    }

    public override void OnConnected()
    {
        base.OnConnected();

        connectionStatus.text = "Connected to Photon!";
        connectionStatus.color = Color.green;
        roomJoinUI.SetActive(true);
        buttonLoadArena.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
        controlPanel.SetActive(true);
        Debug.Log("Disconnected. Please check your Internet connection.");
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            buttonLoadArena.SetActive(true);
            buttonJoinRoom.SetActive(false);
            playerStatus.text = "You are Lobby Leader";
        }
        else
        {
            playerStatus.text = "Connected to Lobby";
        }
    }
}
