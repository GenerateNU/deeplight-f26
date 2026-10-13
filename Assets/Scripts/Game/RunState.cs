

runState

enum: 
start
roomTransition
death
restart



public void startState() {
    set state to start
    set player to active
    set enemies to active
    set camera to follow player
    ect
}


public void roomTransitionState(beforeRoom, afterRoom, tbd) {
    set state to roomTransition
    set player to inactive
    set enemies to inactive
    set camera to follow player
    load new room
    set player to active
    set enemies to active
    set state to start

}


public void deathState() {
    set state to death
    set player to inactive
    set enemies to inactive
    set camera to follow player
    load death screen
    set state to restart
}

public void restartState() {
    set state to restart
    set player to inactive
    set enemies to inactive
    set camera to follow player idk if we need this
    load start screen
    set state to start
}



