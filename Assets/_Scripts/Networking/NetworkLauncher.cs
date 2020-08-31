using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


/* TODO
*
*   - combine join and create room logic
*   - why did it take so long for start game button to disappear for joinin player?
*/


public enum NetworkStatusEnum { Connecting, Connected, Disconnected, Default }
public enum PlayerLobbyStatusEnum { LobbyLeader, JoinedPlayer }

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    [Header("General")]
    [SerializeField] private string gameVersion = "1";
    [SerializeField] private byte maxPlayersPerRoom = 2;

    private string _playerName = "";
    private string _roomName = "";

    [Header("Status")]
    [SerializeField] private Text playerStatus;
    [SerializeField] private Text connectionStatus;
    private bool isConnecting = false;

    /* EVENTS */
    public delegate void NetworkStatusEventHandler(NetworkStatusEnum statusEnum);
    public static event NetworkStatusEventHandler NetworkStatusChanged;

    public delegate void GameInfoEventHandler(string infoMessage);
    public static event GameInfoEventHandler GameInfo;

    public delegate void GameLobbyEventHandler(PlayerLobbyStatusEnum lobbyEnum);
    public static event GameLobbyEventHandler PlayerStatusChanged;

    public delegate void OnlinePlayersEventHandler(string[] OnlinePlayers);
    public static event OnlinePlayersEventHandler OnlinePlayers;

    /* UNITY METHODS */
    private void Awake()
    {
        RegisterEvents();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log(GameConstants.LOG_CONNECTING_TO_PHOTON);

        ConnectToPhoton();
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    /* PHOTON OVERRIDE METHODS */
    public override void OnConnected()
    {
        base.OnConnected();

        NetworkStatusChanged?.Invoke(NetworkStatusEnum.Connected);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        NetworkStatusChanged?.Invoke(NetworkStatusEnum.Disconnected);
        GameInfo?.Invoke(GameConstants.ERROR_DISCONNECTED_FROM_PHOTON);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerStatusChanged?.Invoke(PlayerLobbyStatusEnum.LobbyLeader);
        }
        else
        {
            PlayerStatusChanged?.Invoke(PlayerLobbyStatusEnum.JoinedPlayer);
        }
    }

    /* NETWORK LOGIC */
    private void ConnectToPhoton()
    {
        NetworkStatusChanged?.Invoke(NetworkStatusEnum.Connecting);
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void CreateOrJoinRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = _playerName;
        Debug.Log("PhotonNetwork.IsConnected | Trying to create/Join Room " + _roomName);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        TypedLobby typedLobby = new TypedLobby(_roomName, LobbyType.Default);

        PhotonNetwork.JoinOrCreateRoom(_roomName, roomOptions, typedLobby);
        Debug.Log("online players: " + PhotonNetwork.PlayerList.ToString());
    }

    private void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // TRIGGER EVENT FOR STARTING GAME (IN GAME CONTROLLER?)
            Debug.Log("Game loaded!");
        }
        else
        {
            GameInfo?.Invoke(GameConstants.ERROR_MORE_PLAYERS_NEEDED);
        }
    }

    /* EVENT METHODS */
    private void OnGameRoomCreated()
    {
        if (PhotonNetwork.IsConnected)
        {
            CreateOrJoinRoom();
        }
        else
        {
            // Trigger error
        }
    }

    private void OnGameStarted()
    {
        if (PhotonNetwork.IsConnected)
        {
            StartGame();
        }
        else
        {
            // Trigger error
        }
    }

    private void OnPlayerNameUpdated(string playerName)
    {
        _playerName = playerName;
        Debug.Log("player name: " + playerName);
    }

    private void OnRoomNameUpdated(string roomName)
    {
        _roomName = roomName;
        Debug.Log("room name: " + roomName);
    }

    /* EVENT REGISTRATIONS */
    private void RegisterEvents()
    {
        UIManager.GameRoomCreated += OnGameRoomCreated;
        UIManager.GameStarted += OnGameStarted;

        UIManager.PlayerNameUpdated += OnPlayerNameUpdated;
        UIManager.RoomNameUpdated += OnRoomNameUpdated;
    }

    private void UnRegisterEvents()
    {
        UIManager.GameRoomCreated -= OnGameRoomCreated;
        UIManager.GameStarted -= OnGameStarted;

        UIManager.PlayerNameUpdated -= OnPlayerNameUpdated;
        UIManager.RoomNameUpdated -= OnRoomNameUpdated;
    }
}
