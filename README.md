# VRC-Respawn-Tracker
allows you to track if a remote player or if a local player has respawned

# Setup

1. Download the .unitypackage from the releases section and drag and drop the "RespawnTrackers" prefab into the scene
2. unpack the prefab and look for the "RespawnTracker (Number)" and check your Max. world capacity and either duplicate or remove "RespawnTracker"'s so this way everyone will properly get their own tracker when the world is fully filled otherwise some people will be left without a tracker and will need to wait for someone with a tracker to leave for them to be assigned a tracker!
3. connect your code to the respawntracker in some way eg. ```public RHandler re;``` to use functions properly in a update or frame by frame loop.
4. do whatever you're wanting to do with remote player respawn data. ```example: using "GetRespawnPID" to check if someone else has respawned and remove them from a list without needing to use SetOwner to that remote user and just allowing things to be handled by someone else who either has Lower Ping & High fps rather then giving someone ownership who might be laggy &/ or has very high ping (when someone like that gets ownership it can slow down things and this is just meant to make that faster!)```

# Example Usage

```
--------------------GetRespawnPID--------------------

int[] respawnedPIDs = re.GetRespawnPID();
if (respawnedPIDs.Length > 0 && respawnedPIDs[0] != -1)
    {
    foreach (int pid in respawnedPIDs)
    {
        // logic for all respawning players to be run locally or in a synced manner.
        
        if (pid != Networking.LocalPlayer.playerId)\
        {
            // logic here to ignore the localplayer respawning and only get remote respawns.
        }

        if (pid != 5)\
        {
            // ignores the player with playerId 5 making it where everyone else is tracked besides the 5th player.
        }
    }
}

--------------------GetRespawnName--------------------

string[] respawnedNames = re.GetRespawnName();
if (respawnedNames.Length > 0 && !(respawnedNames.Length == 1 && respawnedNames[0] == "None"))
    {
        foreach (string name in respawnedNames)
        {
            // Logic for all respawning players to be run locally or in a synced manner
            
            if (name != Networking.LocalPlayer.displayName)
            {
                // Logic to ignore the local player respawning and handle only remote players
            }
            
            if (name != "Player5")
            {
                // Ignores anyone with the name Player5 and does logic for anyone else without the name Player5
            }
        }
    }
}

--------------------Both Functions being used together--------------------

int[] respawnedPIDs = re.GetRespawnPID();
string[] respawnedNames = re.GetRespawnName();

if (respawnedNames.Length > 0 && !(respawnedNames.Length == 1 && respawnedNames[0] == "None"))
{
    for (int i = 0; i < respawnedNames.Length; i++)
    {
        string name = respawnedNames[i];
        int pid = i < respawnedPIDs.Length ? respawnedPIDs[i] : -1;
        
        // Logic for all respawning players to be run locally or in a synced manner
        Debug.Log($"Player Respawning: Name = {name}, ID = {pid}");
        
        if (name != Networking.LocalPlayer.displayName)
        {
            // Logic to ignore the local player respawning and handle only remote players
            Debug.Log($"Remote Player Respawning: Name = {name}, ID = {pid}");
        }
        
        if (name != "Player5" && pid != 5)
        {
             // Ignores anyone with the name "Player5" or ID 5, tracks anyone else
             Debug.Log($"Player: Name = {name}, ID = {pid}");
        }
    }
}
-------------------------------------------------------------------------
```

# Notes

its meant to be used in something such as a Update() loop/ a Frame by frame loop otherwise you may lose data but if its slightly slower the frame by frame you may not. (its untested if a slower loop would work.)

this is just because vrchat's native "OnPlayerRespawned()" only returns the localplayer and not ever any remote players and some may want to run logic when a remote player respawns aswell rather then only if a localplayer respawns.

ive tested alot of methods and the only good method that is fast and effective is giving everyone a local tracker (yes this affects the overhead but its very minimal.) either methods was far to slow or would lose data to often so this option of tracking respawns was eventually chosen.

have fun & enjoy!

# Known Bugs

currently no known bugs, if you come across a bug please report it so it can be fixed! <sub><sup>please</sup></sub>

# Documentation

| Function | Description | Return Type | Return Value |
|:---------|:------------|:------------|:-------------|
| GetRespawnedPID() | Get recently respawned player's ID(s) | int-array | playerID(s) or -1 for noone has respawned and 0 is unused |
| GetRespawnedName() | Get recently respawned player's name(s) | string-array | username(s) or empty if noone has respawned recently |

| Setting | Description |
|:---------|:------------|
| DebugMsgs | Defaulted to true, will print debug messsages eg. Remote Player has respawned. Name: Frenetic Furry! ID: 69 / Local Player has respawned. Name: Frenetic Furry! ID: 420
