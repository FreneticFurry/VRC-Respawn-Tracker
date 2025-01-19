using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RTracker : UdonSharpBehaviour
{
    private float resettime;

    [UdonSynced, FieldChangeCallback(nameof(SyncedRespawn))] public bool _HasRespawned;

    private bool SyncedRespawn
    {
        set
        {
            resettime = Time.time + 0.15f;
            _HasRespawned = true;
        }
        get => _HasRespawned;
    }

    private void Start()
    {
        bool[] assignedPIDs = new bool[VRCPlayerApi.GetPlayerCount() + 1];

        foreach (Transform sibling in transform.parent.GetComponentsInChildren<Transform>())
        {
            RTracker tracker = sibling.GetComponent<RTracker>();
            if (tracker == null) continue;

            VRCPlayerApi owner = Networking.GetOwner(tracker.gameObject);
            if (owner != null && owner.IsValid())
            {
                if (!assignedPIDs[owner.playerId])
                {
                    tracker.gameObject.name = $"{owner.playerId}";
                    assignedPIDs[owner.playerId] = true;
                }
                else
                {
                    tracker.gameObject.name = "Unused";
                }
            }
            else
            {
                tracker.gameObject.name = "Unused";
            }
        }

        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null || assignedPIDs[localPlayer.playerId]) return;

        foreach (Transform sibling in transform.parent.GetComponentsInChildren<Transform>())
        {
            RTracker tracker = sibling.GetComponent<RTracker>();
            if (tracker == null || tracker.gameObject.name != "Unused") continue;

            Networking.SetOwner(localPlayer, tracker.gameObject);
            tracker.gameObject.name = $"{localPlayer.playerId}";
            break;
        }
    }

    public void OnPlayerRespawn()
    {
        if (Networking.IsOwner(gameObject))
        {
            if (gameObject.name == "Unused" || gameObject.name != $"{Networking.GetOwner(gameObject).playerId}") return;

            SyncedRespawn = !SyncedRespawn;
            RequestSerialization();
        }
    }

    private void Update()
    {
        if (resettime <= Time.time)
        {
            _HasRespawned = false;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        bool[] assignedPIDs = new bool[VRCPlayerApi.GetPlayerCount() + 1];

        foreach (Transform sibling in transform.parent.GetComponentsInChildren<Transform>())
        {
            RTracker tracker = sibling.GetComponent<RTracker>();
            if (tracker == null) continue;

            VRCPlayerApi owner = Networking.GetOwner(tracker.gameObject);
            if (owner != null && owner.IsValid())
            {
                assignedPIDs[owner.playerId] = true;
            }
        }

        if (!assignedPIDs[player.playerId])
        {
            foreach (Transform sibling in transform.parent.GetComponentsInChildren<Transform>())
            {
                RTracker tracker = sibling.GetComponent<RTracker>();
                if (tracker == null || tracker.gameObject.name != "Unused") continue;

                Networking.SetOwner(player, tracker.gameObject);
                tracker.gameObject.name = $"{player.playerId}";
                return;
            }
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(players);

        foreach (Transform sibling in transform.parent.GetComponentsInChildren<Transform>())
        {
            RTracker tracker = sibling.GetComponent<RTracker>();
            if (tracker == null || tracker.gameObject.name != $"{player.playerId}") continue;

            bool reassigned = false;
            foreach (VRCPlayerApi p in players)
            {
                if (p == null || !p.IsValid()) continue;

                bool hasTracker = false;
                foreach (Transform s in transform.parent.GetComponentsInChildren<Transform>())
                {
                    RTracker t = s.GetComponent<RTracker>();
                    if (t != null && t.gameObject.name == $"{p.playerId}")
                    {
                        hasTracker = true;
                        break;
                    }
                }

                if (!hasTracker)
                {
                    Networking.SetOwner(p, tracker.gameObject);
                    tracker.gameObject.name = $"{p.playerId}";
                    reassigned = true;
                    break;
                }
            }

            if (!reassigned)
            {
                VRCPlayerApi lowest = null;
                int pid = int.MaxValue;

                foreach (VRCPlayerApi p in players)
                {
                    if (p == null || !p.IsValid()) continue;

                    if (p.playerId < pid)
                    {
                        pid = p.playerId;
                        lowest = p;
                    }
                }

                if (lowest != null)
                {
                    Networking.SetOwner(lowest, tracker.gameObject);
                    tracker.gameObject.name = "Unused";
                }
            }
        }
    }
}
