using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _createOrJoinGameRoomCanvas;
    [SerializeField] private GameObject _gameRoomCreatedCanvas;

    [Header("Buttons")]
    [SerializeField] private GameObject _createOrJoinGameRoomButton;
    [SerializeField] private GameObject _startGameButton;
    [SerializeField] private Button _createOrJoinGameMainMenuButton;

    [Header("UI")]
    [SerializeField] private InputField _playerNameField;
    private string _playerName;
    [SerializeField] private InputField _roomNameField;
    private string _roomName;
    [SerializeField] private Text _networkStatusText;
    [SerializeField] private Text _infoStatusText;
    [SerializeField] private Text _roomCreatedRoomNameText;
    [SerializeField] private Text[] _playersOnlineInRoomText;
    private string[] _playersOnlineInRoom;

    /* Events */
    public delegate void GameRoomCreatedEventHandler();
    public static event GameRoomCreatedEventHandler GameRoomCreated;

    public delegate void GameStartedEventHandler();
    public static event GameStartedEventHandler GameStarted;

    public delegate void PlayerNameUpdatedEventHandler(string name);
    public static event PlayerNameUpdatedEventHandler PlayerNameUpdated;

    public delegate void RoomNameUpdatedEventHandler(string name);
    public static event RoomNameUpdatedEventHandler RoomNameUpdated;

    /*  UNITY METHODS   */
    private void Awake()
    {
        RegisterEvents();
    }

    private void Start()
    {
        _infoStatusText.text = GameConstants.LOG_CONNECTING_TO_PHOTON;
        DisableMainMenuButtons();
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    /* HELPER METHODS */
    private void DisableAllCanvases()
    {
        _mainMenuCanvas.SetActive(false);
        _createOrJoinGameRoomCanvas.SetActive(false);
        _gameRoomCreatedCanvas.SetActive(false);
    }

    private void SetCanvasActiveAndDisableOthers(GameObject canvas)
    {
        DisableAllCanvases();
        canvas.SetActive(true);
    }

    private void SetMainMenuButtonsInteractable()
    {
        _createOrJoinGameMainMenuButton.interactable = true;
    }

    private void DisableMainMenuButtons()
    {
        _createOrJoinGameMainMenuButton.interactable = false;
    }

    /*  UI EVENTS */

    public void CreateOrJoinGameButtonClicked()
    {
        SetCanvasActiveAndDisableOthers(_createOrJoinGameRoomCanvas);
        _infoStatusText.text = GameConstants.INFO_CREATE_OR_JOIN_ROOM;
    }

    public void CreateOrJoinRoomButtonClicked()
    {
        // NULL CHECK?
        GameRoomCreated?.Invoke();
        SetCanvasActiveAndDisableOthers(_gameRoomCreatedCanvas);
        _roomCreatedRoomNameText.text = "Room name: " + _roomName;
    }

    public void StartGameButtonClicked()
    {
        GameStarted?.Invoke();
    }

    public void SetPlayerName()
    {
        _playerName = _playerNameField.text;
        PlayerNameUpdated?.Invoke(_playerName);
    }

    public void SetRoomName()
    {
        _roomName = _roomNameField.text;
        RoomNameUpdated?.Invoke(_roomName);
    }

    /*  EVENT METHODS  */
    private void OnNetworkStatusChanged(NetworkStatusEnum statusEnum)
    {
        if (_networkStatusText == null)
        {
            return;
        }
        switch (statusEnum)
        {
            case NetworkStatusEnum.Connecting:
                _networkStatusText.text = GameConstants.STATUS_CONNECTING;
                break;
            case NetworkStatusEnum.Connected:
                _networkStatusText.text = GameConstants.STATUS_CONNECTED;
                _infoStatusText.text = "";
                SetMainMenuButtonsInteractable();
                break;
            case NetworkStatusEnum.Disconnected:
                _networkStatusText.text = GameConstants.STATUS_DISCONNECTED;
                SetCanvasActiveAndDisableOthers(_mainMenuCanvas);
                break;
            default:
                Debug.Log(GameConstants.ERROR_THIS_SHOULDNT_HAVE_HAPPENED);
                break;
        }
    }

    private void OnPlayerStatusChanged(PlayerLobbyStatusEnum playerStatusEnum)
    {
        switch (playerStatusEnum)
        {
            case PlayerLobbyStatusEnum.LobbyLeader:
                _infoStatusText.text = GameConstants.PLAYER_STATUS_LOBBY_LEADER;
                break;
            case PlayerLobbyStatusEnum.JoinedPlayer:
                _infoStatusText.text = GameConstants.PLAYER_STATUS_JOINED_PLAYER;
                _startGameButton.SetActive(false);
                break;
            default:
                Debug.Log(GameConstants.ERROR_THIS_SHOULDNT_HAVE_HAPPENED);
                break;
        }
    }

    private void OnGameInfo(string infoMessage)
    {
        _infoStatusText.text = infoMessage;

        if (infoMessage.Equals(GameConstants.ERROR_DISCONNECTED_FROM_PHOTON))
        {
            SetCanvasActiveAndDisableOthers(_mainMenuCanvas);
        }
    }

    /*  EVENT REGISTRATIONS */
    private void RegisterEvents()
    {
        NetworkLauncher.NetworkStatusChanged += OnNetworkStatusChanged;
        NetworkLauncher.GameInfo += OnGameInfo;
        NetworkLauncher.PlayerStatusChanged += OnPlayerStatusChanged;
    }

    private void UnRegisterEvents()
    {
        NetworkLauncher.NetworkStatusChanged -= OnNetworkStatusChanged;
        NetworkLauncher.PlayerStatusChanged -= OnPlayerStatusChanged;
        NetworkLauncher.GameInfo -= OnGameInfo;
    }
}
