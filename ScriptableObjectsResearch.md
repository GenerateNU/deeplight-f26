# Scriptable Objects

## What They Are

A ScriptableObject is a Unity class for storing data as a standalone asset in your project, independent of any GameObject or scene. By creating a C# class that derives from UnityEngine.ScriptableObject instead of the usual MonoBehaviour, you can define data fields and methods that describe a particular kind of object. Unlike a MonoBehaviour, a ScriptableObject can’t be attached to a GameObject and instead exists as a .asset file that other scripts and prefabs can reference.

Placing the attribute [CreateAssetMenu(fileName = "NewRoom", menuName = "Data/Room")] above the class definition adds an entry to Unity’s Assets > Create menu. Here, fileName is the default name of the new asset and menuName is its path in the menu. Each time you choose that entry, Unity creates a new asset of that class, with fields you can edit in the Inspector.

## Why They’re Useful
Rooms are a good fit for ScriptableObjects. Rather than storing a room’s information inside its prefab’s scripts, you can write one ScriptableObject class that holds the room’s data and create a separate asset for each room. Each asset then references the prefab for that room’s physical layout. This keeps data separate from the layout, so the layout generator can read a room’s information (its category, rarity, and where it can spawn) without having to load the room prefab itself. It also means designers can adjust values in the Inspector without touching code or prefabs.

Because prefabs and scripts hold a reference to a ScriptableObject asset, the data is stored once rather than copied. If a room’s data lived inside a prefab’s scripts, every instance of that prefab would carry its own copy. With a ScriptableObject, every instance points to the same asset.

Changing a ScriptableObject’s values at runtime affects everything that references it. In the Unity Editor, those changes also persist to the asset after you exit Play mode, but in a built game they reset the next time the game launches.

## An Example: Enter the Gungeon

Enter the Gungeon uses a ScriptableObject class to define metadata for each room:
the name of the room
* a custom enum for the room’s main category (normal, boss, reward, etc.)
* a custom enum for a more specific category (combat, trap, hub, or connector)
* a float used by the layout generator to signal the room’s rarity during selection
* a prefab containing a room layout data script, which holds the room’s doors, enemy waves, and other functionality
* a list with the floors the room can spawn on
* a list with predefined directions where doors can be placed to connect to other rooms

I personally wouldn’t change much about this design structure for rooms in our case. I very much like the idea of separating the room’s physical prefab from its metadata. I do, however, think that bringing the enemy waves data outside of the prefab’s own script and onto the ScriptableObject would be beneficial since we might want enemy spawns to be independent and interchangeable between different room layouts.
