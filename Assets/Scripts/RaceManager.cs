using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : NetworkBehaviour
{
    public static RaceManager Instance;

    [Networked] public NetworkBool RaceFinished { get; set; }

    private static List<PlayerMovement> players = new List<PlayerMovement>();

    public Transform CheckPointParent;
    public List<Transform> Checkpoints;

    [SerializeField] StartGreenLightManager StartGreenLightManager;

    public void Awake()
    {
        for (int i = 0; i < CheckPointParent.childCount; i++) {

            Checkpoint currentCheckPoint = this.CheckPointParent.GetChild(i).GetComponent<Checkpoint>();
            if (currentCheckPoint != null) {
                currentCheckPoint.Index = i;
                if(i != 0)
                {
                    currentCheckPoint.gameObject.SetActive(false);
                }
            }
            Checkpoints.Add(this.CheckPointParent.GetChild(i));
        }
    }
    public override void Spawned()
    {
        if (Instance == null) Instance = this;

        if (Object.HasStateAuthority)
        {
            // Clear the results when starting a new session
            RaceResults.Players.Clear();
        }

        players.Clear(); // clear the local player list
    }

    private void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    public void RegisterPlayer(PlayerMovement player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    public int GetPlayerPosition(PlayerMovement localPlayer)
    {
        var sorted = players
            .OrderByDescending(p => p.CurrentCheckpointIndex)
            .ThenBy(p => Vector3.Distance(p.transform.position, Checkpoints[Mathf.Clamp(p.CurrentCheckpointIndex + 1, 0, Checkpoints.Count - 1)].position))
            .ToList();

        return sorted.IndexOf(localPlayer) + 1;
    }

    public int GetTotalPlayers() => players.Count;

    public List<PlayerMovement> GetPlayers() => players;

    public void OnPlayerFinish(PlayerMovement player)
    {
        if (RaceResults.Players.Any(p => p.Name == player.PlayerName))
            return;

        int position = RaceResults.Players.Count + 1;

        RaceResults.Players.Add(new RaceResults.Entry
        {
            Name = player.PlayerName,
            Position = position,
            Time = player.ElapsedTime
        });

        if (Object.HasStateAuthority && RaceResults.Players.Count >= GetTotalPlayers())
        {
            Runner.LoadScene("FinalResultsScene", LoadSceneMode.Single);
        }
    }

    public void StartCountdown()
    {
        StartGreenLightManager.OnCountdownComplete += OnCountdownComplete;
        StartCoroutine(StartGreenLightManager.StartCountdown());
    }

    private void OnCountdownComplete()
    {
        StartGreenLightManager.OnCountdownComplete -= OnCountdownComplete;

        foreach (var player in GetPlayers())
        {
            player.canRun = true;
        }

        Debug.Log("All players can now run!");
    }

}
