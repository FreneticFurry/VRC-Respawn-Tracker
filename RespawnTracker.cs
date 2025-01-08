using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RespawnTracker : UdonSharpBehaviour // if you dont name the .cs file to be RespawnTracker then rename "RespawnTracker" to the .cs file name.
{
    [UdonSynced, FieldChangeCallback(nameof(RespawnedPlayerId))]
    private int _respawnedPlayerId = -1;
    public bool DebugMsgs = true;
    public int RespawnedPlayerId
    {
        set
        {
            _respawnedPlayerId = value;
            if (value != -1)
            {
                if (value == Networking.LocalPlayer.playerId && DebugMsgs)
                {
                    Debug.Log($"Local Player Respawned: ID {value}");
                }
                else
                {
                    Debug.Log($"Remote Player Respawned: ID {value}");
                }
            }
        }
        get => _respawnedPlayerId;
    }

    // Respawn Logic \\

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }

            RespawnedPlayerId = player.playerId;
            RequestSerialization();
            SendCustomEventDelayedSeconds(nameof(ResetPID), 0.175f);
        }
    }

    public void ResetPID()
    {
        if (Networking.IsOwner(gameObject))
        {
            RespawnedPlayerId = -1;
            RequestSerialization();
        }
    }

    // Helper Functions, Recommended functions for use in other code for whatever \\

    public bool GetIfRemote()
    {
        return RespawnedPlayerId >= 0 && RespawnedPlayerId != Networking.LocalPlayer.playerId;
    }

    public int GetRespawnedPID()
    {
        return RespawnedPlayerId;
    }

    public string GetRespawnedName()
    {
        if (RespawnedPlayerId < 0) return string.Empty;

        VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(players);

        foreach (var player in players)
        {
            if (player.playerId == RespawnedPlayerId)
            {
                return player.displayName;
            }
        }

        return string.Empty;
    }
}
