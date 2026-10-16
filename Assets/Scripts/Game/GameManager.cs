using UnityEngine;

/// <summary>
/// A class that manages and delegates state behavior to StateManeger and updates MetaProgression fields.
/// </summary>
public class GameManager : MonoBehaviour 
{
    public StateManeger state; // Reference to the StateManeger component

    /// <summary>
    /// Start is called before the first frame update, sets the run state to start.
    /// </summary>
    void Start()
    {
        StateManeger.setCurrentState(StateManeger.currentState.start);
    }

    /// <summary>
    /// Update is called once per frame, checks the current state and calls the appropriate method in RunState.
    /// </summary>
    update() {
        switch (state.getCurrentState()) {
            case StateManeger.currentState.start:
                state.startState();
                break;
            case StateManeger.currentState.inPlay:
                state.inPlayState();
                break;
            case StateManeger.currentState.roomTransition:
                state.roomTransitionState();
                break;
            case StateManeger.currentState.death:
                state.deathState();
                // add method in meta progression to update run count and other relevant stats
                break;
            case StateManeger.currentState.restart:
                state.restartState();
                break;
            default:
                Debug.LogWarning("Unhandled game state: " + state.getCurrentState());
                break;
        }
    }
}









