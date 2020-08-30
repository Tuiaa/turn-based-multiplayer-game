using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _createGameRoomCanvas;
    [SerializeField] private GameObject _joinGameRoomCanvas;
    [SerializeField] private GameObject _gameRoomCreatedCanvas;

    [Header("Buttons")]
    [SerializeField] private GameObject _createGameRoomButton;
    [SerializeField] private GameObject _joinGameRoomButton;
    [SerializeField] private GameObject _startGameButton;
    [SerializeField] private Button _createGameMainMenuButton;
    [SerializeField] private Button _joinGameMainMenuButton;

    [Header("UI")]
    [SerializeField] private InputField _joinplayerNameField;
    [SerializeField] private InputField _joinroomNameField;
    [SerializeField] private InputField _createplayerNameField;
    [SerializeField] private InputField _createroomNameField;
    [SerializeField] private Text _networkStatusText;
    [SerializeField] private Text _infoStatusText;

    /* Events */
    public delegate void GameRoomCreatedEventHandler();
    public static event GameRoomCreatedEventHandler GameRoomCreated;

    public delegate void GameRoomJoinedEventHandler();
    public static event GameRoomJoinedEventHandler GameRoomJoined;

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
        _createGameRoomCanvas.SetActive(false);
        _joinGameRoomCanvas.SetActive(false);
        _gameRoomCreatedCanvas.SetActive(false);
    }

    private void SetCanvasActiveAndDisableOthers(GameObject canvas)
    {
        DisableAllCanvases();
        canvas.SetActive(true);
    }

    private void SetMainMenuButtonsInteractable()
    {
        _createGameMainMenuButton.interactable = true;
        _joinGameMainMenuButton.interactable = true;
    }

    private void DisableMainMenuButtons()
    {
        _createGameMainMenuButton.interactable = false;
        _joinGameMainMenuButton.interactable = false;
    }

    /*  UI EVENTS */

    public void CreateGameButtonClicked()
    {
        SetCanvasActiveAndDisableOthers(_createGameRoomCanvas);
    }

    public void JoinGameButtonClicked()
    {
        SetCanvasActiveAndDisableOthers(_joinGameRoomCanvas);
    }

    public void CreateRoomButtonClicked()
    {
        // NULL CHECK?
        GameRoomCreated?.Invoke();
        SetCanvasActiveAndDisableOthers(_gameRoomCreatedCanvas);
    }

    public void JoinRoomButtonClicked()
    {
        // NULL CHECK?
        GameRoomJoined?.Invoke();
        SetCanvasActiveAndDisableOthers(_gameRoomCreatedCanvas);
    }

    public void StartGameButtonClicked()
    {
        GameStarted?.Invoke();
    }

    public void SetPlayerNameCreate()
    {
        PlayerNameUpdated?.Invoke(_createplayerNameField.text);
    }

    public void SetPlayerNameJoin()
    {
        PlayerNameUpdated?.Invoke(_joinplayerNameField.text);
    }

    public void SetRoomNameCreate()
    {
        RoomNameUpdated?.Invoke(_createroomNameField.text);
    }

    public void SetRoomNameJoin()
    {
        RoomNameUpdated?.Invoke(_joinroomNameField.text);
    }

    /*  EVENT METHODS  */
    private void OnNetworkStatusChanged(NetworkStatusEnum statusEnum)
    {
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
