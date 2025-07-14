using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public Transform SpawnPositionParent;
    public List<Transform> spawnPositionList = new List<Transform>();
    private List<int> usedSpawnIndices = new List<int>();
    public List<PlayerMovement> PlayerRefList;

    public void Awake()
    {
        for (int i = 0; i < SpawnPositionParent.childCount; i++)
        {
            spawnPositionList.Add(SpawnPositionParent.GetChild(i));
        }
    }
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            int spawnIndex = GetAvailableSpawnIndex();
            if (spawnIndex == -1)
            {
                Debug.LogWarning("No spawn points available!");
                return;
            }

            usedSpawnIndices.Add(spawnIndex);
            Transform spawnPoint = spawnPositionList[spawnIndex];
            PlayerMovement currentPlayerMovement = null;
            Runner.Spawn(
                PlayerPrefab,
                spawnPoint.position,
                spawnPoint.rotation,
                player,
                (runner, obj) =>
                {
                    currentPlayerMovement = obj.GetComponent<PlayerMovement>();
                    // Add uniqueness to the name
                    currentPlayerMovement.PlayerName = $"Player {player.PlayerId}_{Random.Range(1000, 9999)}";
                    this.PlayerRefList.Add(currentPlayerMovement);
                    var ui = FindObjectOfType<UILeaderboard>();
                    if (ui != null)
                        currentPlayerMovement.LeaderboardUI = ui;
                });
            Debug.Log("PlayerRefList.Count " + PlayerRefList.Count);

            if(player.PlayerId != 1)
            {
                currentPlayerMovement.canRun = true;
            }
            else
            {
                RaceManager.Instance.StartCountdown();
            }
        }
    }

    private int GetAvailableSpawnIndex()
    {
        for (int i = 0; i < spawnPositionList.Count; i++)
        {
            if (!usedSpawnIndices.Contains(i))
                return i;
        }
        return -1;
    }
}