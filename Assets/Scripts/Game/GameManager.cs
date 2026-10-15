using UnityEngine;

/// <summary>
/// A class that manages and delegates state behavior to Runstate and updates MetaProgression fields.
/// </summary>
public class GameManager : MonoBehaviour 
{
    public RunState runState; // Reference to the RunState component

    /// <summary>
    /// Start is called before the first frame update, sets the run state to start.
    /// </summary>
    void Start()
    {
        runState.setCurrentState(RunState.currentState.start);
    }

    /// <summary>
    /// Update is called once per frame, checks the current state and calls the appropriate method in RunState.
    /// </summary>
    update() {
        switch (runState.getCurrentState()) {
            case RunState.currentState.start:
                runState.startState();
                break;
            case RunState.currentState.inPlay:
                runState.inPlayState();
                break;
            case RunState.currentState.roomTransition:
                RunState.roomTransitionState();
                break;
            case RunState.currentState.death:
                runState.deathState();
                // add method in meta progression to update run count and other relevant stats
                break;
            case RunState.currentState.restart:
                runState.restartState();
                break;
            default:
                Debug.LogWarning("Unhandled game state: " + runState.getCurrentState());
                break;
        }
    }
}









