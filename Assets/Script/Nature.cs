using UnityEngine;
using System.Collections.Generic;
using TMPro;


public class Nature : MonoBehaviour, IComparer<Player>
{
    public static Nature instance { get; private set; }
    public GameObject playerPrefab;
    public Transform playerPopulation;
    public TextMeshProUGUI GenText;
    public TextMeshProUGUI BestInGenText;
    public TextMeshProUGUI BestYetText;

    public float pickTopPercent = 5;
    public int populationSize;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public NetworkVisual netVis;
    [Tooltip("Mutation Rate between 0 o 100")]
    public float mutationRate = 1f;
    [Tooltip("Move to next population after this many seconds")]
    public float populationLifetime = 10f;

    List<Player> alivePlayers;
    List<Player> deadPlayers;
    float populationTimer;
    int bestScoreInGen = -1;
    int bestScoreYet = -1;
    int genCount = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        alivePlayers = new List<Player>();
        deadPlayers = new List<Player>();
        SpawnPopulation();
        populationTimer = populationLifetime;
        GenText.SetText("Genaration: " + genCount);
    }

    void Update()
    {
        if (populationTimer <= 0)
        {
            NextPopulation();
            populationTimer = populationLifetime;
        }
        else { populationTimer -= Time.deltaTime; }
    }

    public void Kill(Player player)
    {
        alivePlayers.Remove(player);
        deadPlayers.Add(player);
        if (alivePlayers.Count == 0) { NextPopulation(); }
        else if (player.brain == netVis.getNet()) { netVis.setNet(alivePlayers[0].brain); }
    }

    public void TrySetScore(float score)
    {
        if (score > bestScoreInGen)
        {
            bestScoreInGen = (int) score;
            BestInGenText.SetText("Best in Gen: " + bestScoreInGen);
        }
    }

    private void SpawnPopulation() {
        for (int i=0; i < populationSize; i++)
        {
            Player player = Instantiate(playerPrefab, startPoint, Quaternion.identity, playerPopulation).GetComponent<Player>();
            player.brain = new Network(ObstacleManager.instance.obstacleCount + 4, 10, 8);
            alivePlayers.Add(player);
        }
        netVis.setNet(alivePlayers[0].brain);
    }

    private Player NormalizeFitnesess()
    {
        // Normalizes fitness value of all players to fit it between 0 and 1, and returns the player with best fitness
        float maxFitness = float.MinValue;
        Player bestOne = null;

        foreach (Player player in deadPlayers)
        {
            if (player.evaluateFitness() > maxFitness)
            {
                maxFitness = player.fitness;
                bestOne = player;
            }
        }

        foreach (Player player in deadPlayers)
        {
            player.fitness /= maxFitness;
        }
        return bestOne;
    }

    private Player PickOne() {
        // Selects and returns one player at random based on fitness score
        // assumption:  0 <= fitness <= 1
        int index = 0;
        float r = Random.Range(0f, 1f);
        
        while (r > 0) {
            r -= deadPlayers[index].fitness;
            index++; }
        return deadPlayers[--index];
    }

    private void NextPopulation() {
        // Add the players which are still alive to dead player
        foreach (Player player in alivePlayers) { deadPlayers.Add(player); }
        alivePlayers.Clear();

        List<Player> playersToKill = new List<Player>();
        foreach (Player pl in deadPlayers)
        {
            if (pl.fitness <= 150)
            {
                playersToKill.Add(pl);
            }
        }
        Debug.Log("Killing: " + playersToKill.Count + " Players");
        foreach (Player pl in playersToKill)
        {
            Destroy(pl.gameObject);
            deadPlayers.Remove(pl);
        }

        // Normalize all fitness score so it remains between 0 and 1. Required for PickOne function.
        NormalizeFitnesess();

        // Create New Populatoin
        for (int i = 0; i < populationSize; i++) {
            Player parent = PickOne();
            Player child = Instantiate(playerPrefab, startPoint, Quaternion.identity, playerPopulation).GetComponent<Player>();
            child.brain = parent.brain.MutatedCopy(mutationRate / 100);
            child.SetColor(parent.GetColor());
            alivePlayers.Add(child);
        }

        // Destroy old Population
        foreach (Player player in deadPlayers) { Destroy(player.gameObject); }
        deadPlayers.Clear();

        // Update best score
        if (bestScoreInGen > bestScoreYet)
        {
            bestScoreYet = bestScoreInGen;
            BestYetText.SetText("Scored " + bestScoreYet + " at Gen " + genCount);
        }
        bestScoreInGen = 0;
        genCount++;
        GenText.SetText("Genaration: " + genCount);
        // Update network visuals
        netVis.setNet(alivePlayers[0].brain);
    }

    public int Compare(Player x, Player y) {
        return (int) ((y.fitness - x.fitness) * populationSize * 2);
    }
}
