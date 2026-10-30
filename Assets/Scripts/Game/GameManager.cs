using UnityEngine;

/// <summary>
/// A class that manages and delegates state behavior to StateManeger and updates MetaProgression fields.
/// </summary>
public class GameManager : MonoBehaviour
{
    public StateManager stateManager; // Reference to the StateManager component
    [SerializeField] private GameObject currentRoom; // TODO: Change to scripted object later
    [SerializeField] private GameObject nextRoom;
    [SerializeField] private MetaProgression metaProgression;
    private GameState currentState;
    private bool firstPlayThrough = false;


    /// <summary>
    /// Start is called before the first frame update, sets the run state to start.
    /// </summary>
    void Start()
    {
        stateManager.SetState(GameState.mainMenu);

        // TODO, how will we set the current and next room for this class...we get it from StateManager ?
    }

    /// <summary>
    /// Update is called once per frame, checks the current state and calls the appropriate method in RunState.
    /// </summary>
    void Update()
    {
        //TODO: connect this functionality to a void onPuase function, put it into a different class called InputSystem with the other types of inputs in player input. 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Methods called by other scripts:

    /// <summary>
    /// Starts the run after pressing the Main menu play button.
    /// </summary>
    public void StartRun()
    {
        stateManager.SetState(GameState.loading);
    }

    /// <summary>
    /// Starts the tutorial after pressing the tutorial button.
    /// </summary>
    public void StartTutorial()
    {
        stateManager.SetState(GameState.tutorial);
    }

    /// <summary>
    /// Called when loading has finished.
    /// </summary>
    public void FinishedLoading()
    {
        stateManager.SetState(GameState.inPlay);
    }

    /// <summary>
    /// A method that sets the next room for the game.
    /// Is called by a door between rooms.
    /// </summary>
    /// <param name="room"></param>
    public void SetUpRoomTransition(GameObject currentRoom, GameObject nextRoom)
    {
        this.currentRoom = currentRoom;
        this.nextRoom = nextRoom;
    }

    /// <summary>
    /// A method that is called when the player dies.
    /// Should be called by the player's health script.
    /// </summary>
    public void PlayerDied()
    {
        stateManager.SetState(GameState.death);
        metaProgression.RecordLoss();
    }

    /// <summary>
    /// A method that is called when the player defeats the game.
    /// Called by the final room's exit.
    /// </summary>
    public void PlayerWonGame()
    {
        stateManager.SetState(GameState.win);
        metaProgression.RecordWin();
    }

    /// <summary>
    /// A method that is called when the player dies/wins the game and goes back to the menu.
    /// </summary>
    public void ReturnToMainMenu()
    {
        stateManager.SetState(GameState.mainMenu);
    }

    /// <summary>
    /// A method that is called when an enemy dies.
    /// </summary>
    public void EnemyKilled()
    {
        metaProgression.RecordEnemyKill();
    }

    /// <summary>
    /// A method that either pauses or unpauses the game depending on the current state.
    /// </summary>
    private void TogglePause()
    {
        GameState current = stateManager.GetCurrentState();

        if (current == GameState.inPlay)
            stateManager.SetState(GameState.pause);
        else if (current == GameState.pause)
            stateManager.SetState(GameState.inPlay);
    }
}
