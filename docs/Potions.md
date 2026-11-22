# Potions

Relevant files: AllPotions, Potion, PotionDropper, PotionPickup, Stat, StatHandler, TempDebugPotionSpawner

Requirements: Player needs a Player tag, a StatHandler and a Rigidbody. The Script needs to access the stat though GetStat for the effects to be applied.

## Adding a Potion
- In the inspector there's an AllPotions asset in Assets/Resources/AllPotions
- Add an element and configure:
  - Stat: What stat the potion affects
  - Ranges: There should be 4 entries. These are the effects based on rarity in order: Common, Rare, UltraRare, Mythic. If the effect should be the same for each rarity, the min and max for each entry can be the same.
  - Operation: What happens to the stat. Add adds the rolled value to the base. Multiply applies 1 + value, so a value of 1 means ×2.
- Assign the RngStreams asset in Assets/GameAssets/RngStreams

## Using Potions in Game

### Placing a Specific Potion
- In Assets/Prefabs/Pick-ups/PotionPickups there are potion prefabs you can drop directly into the scene. You can duplicate the PotionPickup prefab and edit the stat, rarity and operation to match an entry in the AllPotions asset. It has to match an entry in the AllPotions asset to work.

### Random Drops Based on an Event
For breakables and enemy deaths that should drop a random potion
- Call PotionDropper.OnDeath() for a chance that a potion spawns. It will pick an entry in AllPotions and spawn a potion based on the chanceOfDrop in PotionDropper. Call Drop() if the drop should be guaranteed. rarityWeights controls how often each rarity drops
- Add PotionDropper to the object and assign the potion pickup prefab and RngStreamsConfig

## For Testing
- There is a TempDebugPotionSpawner for testing without using a breakable or death. To use this add an empty game object with the PotionDropper script, and an empty game object with the TempDebugPotionSpawner script. Make sure to drag everything necessary in the inspector.
- In game you can then press Q to spawn a random potion pickup.
- This can be deleted once real breakables exist or kept for testing.
