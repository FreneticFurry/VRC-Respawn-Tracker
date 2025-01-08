# VRC-Respawn-Tracker
allows you to track if a remote player or if a local player has respawned

# Example Usages

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
