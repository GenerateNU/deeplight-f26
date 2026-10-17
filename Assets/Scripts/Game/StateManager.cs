using UnityEngine;

/// <summary>
/// A class that manages the current state of the game, including transitions between menu, tutorial,
/// loading, play, room transitions, win, and death.
/// </summary>
public class StateManager : MonoBehaviour 
{

    // todo: make seperate enum class???
    /// Enum representing the different states of the game
    public enum currentState
    {
        mainMenu,
        pause, 
        tutorial, 
        loading,   
        inPlay,
        roomTransition, 
        win,   
        death  
    }

    private currentState state = currentState.mainMenu;



    /// <summary>
    /// Sets the game to the main menu state.
    /// </summary>
    public void mainMenuState()
    {
        state = currentState.mainMenu;
        // set player to inactive
        // set enemies to inactive
        // load main menu screen
    }

    /// <summary>   
    /// Sets the game to the pause state.
    /// </summary>
    public void pauseState()
    {
        state = currentState.pause;
        // freeze player and enemies (e.g. Time.timeScale = 0f)
        // show pause menu
    }


    /// <summary>
    /// Sets the game to the tutorial state.
    /// </summary>
    public void tutorialState()
    {
        state = currentState.tutorial;
        // load tutorial room
        // set player to active
        // show tutorial prompts
    }

    /// <summary>
    /// Sets the game to the loading state.
    /// </summary>
    public void loadingState()
    {
        state = currentState.loading;
        // set player to inactive
        // set enemies to inactive
        // show loading screen
    }

    /// <summary>
    /// Sets the game to the in play state.
    /// </summary>
    public void inPlayState()
    {
        state = currentState.inPlay;
        // unfreeze time (e.g. Time.timeScale = 1f)
        // set player to active
        // set enemies to active
        // set camera to follow player
    }

    /// <summary>
    /// Handles the transition between rooms.
    /// </summary>
    public void roomTransitionState(GameObject beforeRoom, GameObject afterRoom)
    {
        state = currentState.roomTransition;
        // set player to inactive
        // set enemies to inactive
        // set player to active
        // set enemies to active
        // then call inPlayState() in a conditional(player has entered the new room) 

        // no need to worry about loading rooms since it should be preloaded before the game starts

    }


    //// <summary>
    /// Sets the game to the win state.
    /// </summary>
    public void winState()
    {
        state = currentState.win;
        // set enemies to inactive
        // load win screen
    }

    /// <summary>
    /// Handles the death state of the game.
    /// </summary>
    public void deathState()
    {
        state = currentState.death;
        // set enemies to inactive
        // load death screen
    }


    /// <summary>
    /// Restarts the game by returning to the main menu (there is no restart state in the enum).
    /// </summary>
    public void restartState()
    {
        // reset run data (deck, lives, stage) here
        mainMenuState();
    }

    /// <summary>
    /// Gets the current state of the game.     
    /// </summary>
    public void getCurrentState() {
        return state;
    }

    /// <summary>
    /// Sets the current state of the game to the specified new state.
    /// </summary>
    public void setCurrentState(currentState newState) {
        state = newState;
    }
}


