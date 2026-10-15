using UnityEngine;

/// <summary>
/// A class that manages the current state of the game, including transitions between different states such as start, in play, room transition, death, and restart.
/// </summary>
public class Runstate : MonoBehaviour 
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

    currentState state;

    /// <summary>
    /// Start is called before the first frame update, initializes the game state to start.
    /// </summary>
    public void startState() {
        
        // set state to start
        state = currentState.start;

        // set player to active
        // set enemies to active
        // set camera to follow player
        // ect

    }

    /// <summary>
    /// Handles the transition between rooms, including setting the game state to roomTransition and managing the activation and deactivation of the player, enemies, and camera.
    /// </summary>
    public void roomTransitionState(beforeRoom, afterRoom, tbd) {
        // set state to roomTransition
        currentState = currentState.roomTransition;

        // set player to inactive
        // set enemies to inactive
        // set camera to follow player
        // load new room
        // set player to active
        // set enemies to active
        // set state to start

    }

    /// <summary>
    /// Handles the death state of the game, including setting the game state to death and managing the activation and deactivation of the player, enemies, and camera, as well as loading the death screen.
    /// </summary>
    public void deathState() {
        // set state to death
        currentState = currentState.death;
        // set enemies to inactive
        // set camera to follow player
        // load death screen
    }

    /// <summary>
    /// Handles the restart state of the game, including setting the game state to restart and managing the activation and deactivation of the player, enemies, and camera, as well as loading the start screen.
    /// </summary>
    public void restartState() {
        // set state to restart
        currentState = currentState.restart;
        // set player to inactive
        // set enemies to inactive
        // set camera to follow player idk if we need this
        // load start screen
        // set state to start
    }

    /// <summary>
    /// Handles the in play state of the game, including setting the game state to inPlay and managing the activation and deactivation of the player, enemies, and camera.
    /// </summary>
    public void inPlayState() {
        // set state to inPlay
        currentState = currentState.inPlay;
        // set player to active
        // set enemies to active
        // set camera to follow player
    }

    /// <summary>
    /// Gets the current state of the game.     
    /// </summary>
    public void getCurrentState() {
        return currentState;
    }

    /// <summary>
    /// Sets the current state of the game to the specified new state.
    /// </summary>
    public void setCurrentState(currentState newState) {
        currentState = newState;
    }
}


