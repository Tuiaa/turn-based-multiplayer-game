# turn-based-multiplayer-game
Tutorial on how to create a turn-based multiplayer game (still WIP). More detailed tutorial coming when the code is finished.

#### Topics
- How to create a menu for a game
- How to create a game room lobby
- How to join a game room lobby
- How to sync the game using Photon networking library for Unity

# Creating a turn-based multiplayer game with Unity

## How to create a game menu
### Overview of different components

#### UIManager
- Takes care of updating UI using events
- Updates canvases depending on what's the status of NetworkManager

#### NetworkManager
- Handles Photon network connection
- Connects to Photon network, creates or joins a room and starts a game

#### UI Canvases
- Different menus (main menu, join/create room, room created) each has their own canvas
- There's also static canvases that are always set active (network status display canvas, background image canvas, game info status canvas)

### TODO
- Refactor current UI code so it's not all in one huge file
- Add more null checks
- Some smaller modifications which just make the game playability better (grey out join game button until Photon network has been connected etc)
- Add multiplayer to actual game
