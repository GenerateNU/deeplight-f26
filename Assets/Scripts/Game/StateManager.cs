using UnityEngine;

/// <summary>
/// A class that manages the current state of the game, including transitions between menu, tutorial,
/// loading, play, room transitions, win, and death.
/// </summary>
public class StateManager : MonoBehaviour
{
    private GameState state = GameState.mainMenu;

    /// <summary>
    /// Sets the game to the main menu state.
    /// </summary>
    public void MainMenuState()
    {
        state = GameState.mainMenu;
        // set player to inactive
        // set enemies to inactive
        // load main menu screen
    }

    /// <summary>   
    /// Sets the game to the pause state.
    /// </summary>
    public void PauseState()
    {
        state = GameState.pause;
        // freeze player and enemies (e.g. Time.timeScale = 0f)
        // show pause menu
    }


    /// <summary>
    /// Sets the game to the tutorial state.
    /// </summary>
    public void TutorialState()
    {
        state = GameState.tutorial;
        // load tutorial room
        // set player to active
        // show tutorial prompts
    }

    /// <summary>
    /// Sets the game to the loading state.
    /// </summary>
    public void LoadingState()
    {
        state = GameState.loading;
        // set player to inactive
        // set enemies to inactive
        // show loading screen
    }

    /// <summary>
    /// Sets the game to the in play state.
    /// </summary>
    public void InPlayState()
    {
        state = GameState.inPlay;
        // unfreeze time (e.g. Time.timeScale = 1f)
        // set player to active
        // set enemies to active
        // set camera to follow player
    }

    /// <summary>
    /// Handles the transition between rooms.
    /// </summary>
    public void RoomTransitionState(GameObject beforeRoom, GameObject afterRoom)
    {
        state = GameState.roomTransition;
        // set player to inactive
        // set enemies to inactive
        // set player to active
        // set enemies to active
        // then call inPlayState() in a conditional(player has entered the new room) 

        // no need to worry about loading rooms since it should be preloaded before the game starts

    }

    /// <summary>
    /// Sets the game to the win state.
    /// </summary>
    public void WinState()
    {
        state = GameState.win;
        // set enemies to inactive
        // load win screen
    }

    /// <summary>
    /// Handles the death state of the game.
    /// </summary>
    public void DeathState()
    {
        state = GameState.death;
        // set enemies to inactive
        // load death screen
    }

    /// <summary>
    /// Restarts the game by returning to the main menu (there is no restart state in the enum).
    /// </summary>
    public void RestartState()
    {
        // reset run data (deck, lives, stage) here
        MainMenuState();
    }

    /// <summary>
    /// Gets the current state of the game.     
    /// </summary>
    public GameState GetCurrentState()
    {
        return state;
    }

    /// <summary>
    /// Sets the current state of the game to the specified new state.
    /// </summary>
    public void SetState(GameState newState)
    {
        state = newState;

        //Switch case for each state to call corresponding method
        switch (state)
        {
            case GameState.mainMenu:
                MainMenuState();
                break;
            case GameState.pause:
                PauseState();
                break;
            case GameState.tutorial:
                TutorialState();
                break;
            case GameState.loading:
                LoadingState();
                break;
            case GameState.inPlay:
                InPlayState();
                break;
            case GameState.roomTransition:
                RoomTransitionState();
                break;
            case GameState.win:
                WinState();
                break;
            case GameState.death:
                DeathState();
                break;
        }
    }