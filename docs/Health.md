__Health__

Relevant files: HealthComponent, DestroyOnDeath, HealthBarUI, HealthPickup

__Health Component__ contains the API for any game object with health (player, enemies, destructibles, etc)
 - Health is an integer HP clamped between 0 and the maxHealth
 - The current health and max health of the game object are stored as variables
 - The public API exposes: Damage(int), Heal(int), SetMaxHealth, GetCurrentHealth & GetMaxHealth.
 - The API contains two events: OnHealthChanged(current, max) and OnDeath. Scripts that need to trigger based on any changes to current health, max health, or death of the game object should have an OnEnable method and OnDisable method that subscribe and unsubscribe to the respective events.

 __DestroyOnDeath__
 - For a game object to be destroyed after death, it must have a HealthComponent script and DestroyOnDeath script attached.

 __HealthBarUI__ Updates the health bar by listening to the OnHealthChanged(current, max) event.
 - Attach this script to the HealthBar or HealthBarSlider components on the game object. You will need to drag the game object into the health slot, and the HealthBarSlider into the slider slot.

 __HealthPickup__ Heals the player on collision.
  - There is a health pick-up prefab that can be used.
  - To create a different health pick-up, create a game object with a collider and turn on "Is Trigger". Then attach the HealthPickup script.
  - Stays on the ground if player is at full health.

