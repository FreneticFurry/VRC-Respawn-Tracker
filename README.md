# VRC-Respawn-Tracker
allows you to track if a remote player or if a local player has respawned

# Example Usage

```
int checkRespawn = re.GetRespawnedPID(); // can also be GetRespawnedName() instead of PlayerID
if (checkRespawn >= 0)
{
    // logic for player respawning both local and remote.
}

if (checkRespawn >= 0 && re.GetIfRemote())
{
    // only run logic on players that're remote that have just respawned (!& for only localplayer respawn logic)
}
```

# Notes

its meant to be used in something such as a Update() loop/ a Frame by frame loop otherwise you may lose data but if its slightly slower the frame by frame you may not. (its untested if a slower loop would work.)

this is just because vrchat's native "OnPlayerRespawned()" only returns the localplayer and not ever any remote players and some may want to run logic when a remote player respawns aswell rather then only if a localplayer respawns.

have fun & enjoy!

# Documentation

| Function | Description | Return Type | Return Value |
|:---------|:------------|:------------|:-------------|
| ResetPID() | Force reset tracked playerID (internal use) | void | force resets the PID to -1 |
| GetIfRemote() | Check if respawning player is remote | bool | true = remote, false = local |
| GetRespawnedPID() | Get recently respawned player's ID | int | playerID or -1 for noone has respawned and 0 is unused |
| GetRespawnedName() | Get recently respawned player's name | string | username or empty if noone has respawned recently |
