using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class RHandler : UdonSharpBehaviour
{
    public bool DebugMsgs = true;
    private int[] respawnedPIDs;

    private void Start()
    {
        respawnedPIDs = new int[10];
        for (int i = 0; i < respawnedPIDs.Length; i++) respawnedPIDs[i] = -1;
    }

    private void Update()
    {
        int[] tempRespawnedPIDs = new int[0];
        int count = 0;

        foreach (RTracker rTracker in GetComponentsInChildren<RTracker>())
        {
            if (rTracker == null) continue;

            VRCPlayerApi owner = Networking.GetOwner(rTracker.gameObject);
            int ownerPID = owner != null ? owner.playerId : -1;

            if (owner == null || !rTracker._HasRespawned) continue;

            int[] newArray = new int[count + 1];
            for (int i = 0; i < count; i++)
            {
                newArray[i] = tempRespawnedPIDs[i];
            }
            newArray[count] = ownerPID;
            tempRespawnedPIDs = newArray;
            count++;
        }

        bool hasChanged = respawnedPIDs.Length != tempRespawnedPIDs.Length;
        if (!hasChanged)
        {
            for (int i = 0; i < respawnedPIDs.Length; i++)
            {
                if (respawnedPIDs[i] != tempRespawnedPIDs[i])
                {
                    hasChanged = true;
                    break;
                }
            }
        }

        if (hasChanged)
        {
            respawnedPIDs = tempRespawnedPIDs;

            if (DebugMsgs)
            {
                int[] activePIDs = GetRespawnPID();
                string[] activeNames = GetRespawnName();

                for (int i = 0; i < activePIDs.Length; i++)
                {
                    int pid = activePIDs[i];
                    string name = i < activeNames.Length ? activeNames[i] : "Unknown";

                    if (pid == -1) continue;

                    if (Networking.LocalPlayer != null && pid == Networking.LocalPlayer.playerId)
                    {
                        Debug.Log($"[<color=#fb00ff>Respawn Tracker</color>]: Local Player has respawned. Name: <color=#a34e4e>{name}</color> ID: <color=#cfcf76>{pid}</color>");
                    }
                    else
                    {
                        Debug.Log($"[<color=#fb00ff>Respawn Tracker</color>]: Remote Player has respawned. Name: <color=#a34e4e>{name}</color> ID: <color=#cfcf76>{pid}</color>");
                    }
                }
            }
        }
    }

    public int[] GetRespawnPID()
    {
        if (respawnedPIDs == null)
        {
            return new int[] { -1 };
        }

        int count = 0;
        foreach (int pid in respawnedPIDs) if (pid != -1) count++;
        if (count == 0) return new int[] { -1 };

        int[] result = new int[count];
        int index = 0;
        foreach (int pid in respawnedPIDs)
        {
            if (pid != -1) result[index++] = pid;
        }
        return result;
    }

    public string[] GetRespawnName()
    {
        int[] activePIDs = GetRespawnPID();
        if (activePIDs.Length == 1 && activePIDs[0] == -1) return new string[0];

        string[] names = new string[activePIDs.Length];
        for (int i = 0; i < activePIDs.Length; i++)
        {
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(activePIDs[i]);
            names[i] = player != null ? player.displayName : " ";
        }
        return names;
    }
}
