using System.Collections;
using System.Collections.Generic;

public class GameConstants
{
    /* Network */
    public const string LOG_CONNECTING_TO_PHOTON = "Connecting to Photon network.";
    public const string STATUS_CONNECTING = "Connecting...";
    public const string STATUS_CONNECTED = "Connected!";
    public const string STATUS_DISCONNECTED = "Disconnected.";
    public const string PLAYER_STATUS_LOBBY_LEADER = "You are a lobby leader!";
    public const string PLAYER_STATUS_JOINED_PLAYER = "You joined someone elses room! Wait for lobby leader to start the game!";

    /* Error Messages */
    public const string ERROR_MORE_PLAYERS_NEEDED = "Someone else needs to join the room before the game can be started!";
    public const string ERROR_DISCONNECTED_FROM_PHOTON = "Disconnected. Check your Internet connection.";
    public const string ERROR_THIS_SHOULDNT_HAVE_HAPPENED = "This shouldn't have happened.";

    /* Info texts */
    public const string INFO_CREATE_OR_JOIN_ROOM = "Either create new room by giving it a unique name or connect to excisting room by writing its name.";
}
