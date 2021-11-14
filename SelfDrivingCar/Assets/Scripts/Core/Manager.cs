using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;

public class Manager : MonoBehaviour
{
    // Network settings
    public int inputs = 5;
    public int[] hidden = new int[] { 4, 3 };
    public int outputs = 2;

    public float mutationStrength = 0.2f;
    public float mutationChance = 0.05f;
    public int populationSize = 20;
    public int parentPopulationSize = 2;

    public float mutationReshuffleStrength = 5f;
    public float mutationReshuffleChance = 0.5f;

    // Saved/loaded record instance
    NetworkSettings networkSettings;

    // Network setting for simulation
    public bool trainingEnabled;

    // Best net from current session
    public TestResults bestNet = new TestResults() { fitness = - 999 };

    // Agent settings
    public GameObject playerPrefab;
    public Vector3 startingPosition;
    public int yRotation;
    private GameObject[] players;
    private List<TestResults> results;

    // Generation
    public bool autoStartNextGen = true;
    public bool isGenerationOver = false;
    public bool generateNext = false;
    public bool reshuffleNextGeneration = false;
    public int generation = 0;

    // Checkpoints
    public GameObject checkpointContainer;
    public bool showCheckpoints = false;
        
    // Simulation speed
    public float simulationTimeScale = 1;
    
    // UI
    public TMPro.TMP_Text generationText;

    // Start is called before the first frame update 
    void Start() // TO UPDATE
    {
        DB_Controller.DumpDeepNetRecord(FindObjectOfType<NetworkSettings>().deepNetRecord);
        // Shows or hides checkpoints
        MeshRenderer[] checkpoints = checkpointContainer.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in checkpoints)
        {
            meshRenderer.enabled = showCheckpoints;
        }

        // Checks whenever it should load a network
        LoadNetwork();

        // If the network is brand new a random population is generated and updates generation info
        if (generation == 0)
        {
            GenerateRandomPopulation(populationSize * 3);
            UpdateGenerationInfo();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (autoStartNextGen)
        {
            if (isGenerationOver)
            {
                GenerateNextGeneration();
            }
        }
        else
        {
            if (generateNext)
            {
                if (isGenerationOver)
                {
                    GenerateNextGeneration();
                }
            }
        }
    }

    private void GenerateNextGeneration()
    {
        // Generates next generation based on the best networks from previous generation
        TestResults[] topNets = GetTop(parentPopulationSize);
        if (topNets[0].fitness > bestNet.fitness)
        {
            bestNet = topNets[0];
        }

        GeneratePopulation(populationSize, topNets);
        isGenerationOver = false;
        generateNext = false;
    }

    public void GeneratePopulation(int populationSize, TestResults[] parents)
    {
        // Generates a population with size of populationSize
        // The individual player/agents will be a mutated version of their parents
        // The population will be split evenly split between each parent in parents array
        // In a case where the population size is not a multiple of parents in parents array
        // the population size will be round dawn for each parent and remanding player will be generated base on the first parent
        // e.g population size 71 with 3 parents in parent array GeneratePopulation(71, {Parent_1, Parent_2, Parent_3})
        // since 71/3 = 23.666666667 so you cant evenly split the population between the parents
        // so parent 1 will have 25 children, parent 2 23 and parent 3 23. So that all the children add up to population size

        // Creates a array to hold players/agents
        players = new GameObject[populationSize];
        // Creates a list to store results
        results = new List<TestResults>();

        // Calculates how many children each parent should have
        int[] childrenPerParent = new int[parents.Length];
        int highestCommonFactor = (int)System.Math.Truncate((double)populationSize / parents.Length);
        int reminder = populationSize % highestCommonFactor;
        childrenPerParent[0] = highestCommonFactor + reminder;
        for (int i = 1; i < childrenPerParent.Length; i++)
        {
            childrenPerParent[i] = highestCommonFactor;
        }

        // cumulativeIndex keeps truck of the index for the next agent in the players array
        int cumulativeIndex = 0;
        // For each children in childrenPerParent
        for (int i = 0; i < childrenPerParent.Length; i++)
        {
            // For each child in children
            for (int j = 0; j < childrenPerParent[i]; j++)
            {
                // Generates the player
                double[] wheights = new double[parents[i].wheight.Length];
                double[] biases = new double[parents[i].biases.Length];
                System.Array.Copy(parents[i].wheight, wheights, parents[i].wheight.Length);
                System.Array.Copy(parents[i].biases, biases, parents[i].biases.Length);
                GeneratePlayer(j + cumulativeIndex, wheights, biases);             
            }

            // updates cumulativeIndex
            cumulativeIndex += childrenPerParent[i];
        }

        // Disable genetic reshuffle for the further generations
        reshuffleNextGeneration = false;
        // Updates generation info
        UpdateGenerationInfo();
    }

    public void GenerateRandomPopulation(int populationSize)
    {
        // Creates a array to hold players/agents
        players = new GameObject[populationSize];
        // Creates a list to store results
        results = new List<TestResults>();
        
        // Calculates number of weights
        int nw = DeepNet.NumWeights(inputs, hidden, outputs)[0];
        // Calculates number of biases
        int nb = DeepNet.NumWeights(inputs, hidden, outputs)[1];

        // For each player/agent
        for (int i = 0; i < populationSize; i++)
        {
            // Create array to store wights for the current player/agent 
            double[] w = new double[nw];
            // Create array to store biases for the current player/agent 
            double[] b = new double[nb];
            
            // Sets all the weight to a random value between -10 and 10
            for (int j = 0; j < w.Length; j++)
            {
                w[j] = UnityEngine.Random.Range(-10f, 10f);
            }
            // Sets all the biases to a random value between -10 and 10
            for (int j = 0; j < b.Length; j++)
            {
                b[j] = UnityEngine.Random.Range(-10f, 10f);
            }

            // Creates the player with randomly generate weights and biases
            GeneratePlayer(i, w, b);
        }
    }
    
    public TestResults[] GetTop(int n)
    {
        // Returns n networks with best finiteness from results list

        TestResults[] topNetworks = new TestResults[n];

        foreach(TestResults result in results)
        {
            for (int i = 0; i < n; i++)
            {
                if (result.fitness > topNetworks[i].fitness)
                {
                    // Shifts all values by 1 start at index i
                    for(int j = 0; j < n-i-1; j++)
                    {
                        topNetworks[n - 1 - j] = topNetworks[n - 2 - j];
                    }

                    topNetworks[i] = result;
                    break;
                }
            }
        }

        return topNetworks;
    }

    public bool IsPopulationOver()
    {
        // Returns true is all agents have finished their evaluation/test
        return players.Length == results.Count;
    }

    public void GeneratePlayer(int index, double[] iw, double[] ib)
    {
        // Create the player/agent
        GameObject player = Instantiate(playerPrefab);
        players[index] = player;

        // Sets initial starting position for the player;
        player.GetComponent<Transform>().position = startingPosition;

        // Sets player rotation
        player.GetComponent<Transform>().rotation = Quaternion.Euler(0, yRotation, 0);

        // Sets y-rotation for the player
        player.GetComponent<PlayerMovement>().yRotation = yRotation;

        // Sets player manager settings
        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        playerManager.SetIndex(index);
        playerManager.InitDeepNet(inputs, hidden, outputs, iw, ib);
        if (reshuffleNextGeneration)
        {
            // If reshuffleNextGeneration the player us mutated using different change and strength
            playerManager.Mutate(mutationReshuffleChance, mutationReshuffleStrength);
        }
        else
        {
            // If not reshuffleNextGeneration player is mutated using normal chance and strength
            playerManager.Mutate(mutationChance, mutationStrength);
        }
    }

    public void EndTest(int index, float fitness, double[] w, double[] b, DeepNetMutate deepNetMutate)
    {
        // This function is a callback for agents
        // They call the function after their evaluation is finished
        // e.g after they crash (hit a wall)

        // Destroys the agent after that have finished their test
        Destroy(players[index]);

        // Create a test result for the agent
        TestResults result = new TestResults
        {
            fitness = fitness,
            wheight = w,
            biases = b,
            deepNet = deepNetMutate
        };

        // Adds the result to results array
        results.Add(result);

        // Check if population is over / all agents from the population have finished their evaluation (test)
        if (IsPopulationOver())
        {
            isGenerationOver = true;

            // TEST START 
            TestResults[] topNets = GetTop(2);
            Debug.Log($"Best fitness: {topNets[0].fitness}, Generation: {generation}, Second best fitness: {topNets[1].fitness}, Generation: { generation}");
            // TEST END
        }
    }

    public void UpdateGenerationInfo()
    {
        // Updates generation count
        if (trainingEnabled)
        {
            generation++;
        }
        // Updates generation count UI
        generationText.text = $"Generation {generation}";
    }

    public void LoadNetwork() // TO UPDATE
    {
        // Finds the network setting
        networkSettings = FindObjectOfType<NetworkSettings>();

        // Sets network setting for simulation
        trainingEnabled = networkSettings.train;

        // Sets generation count
        generation = networkSettings.deepNetRecord.generation;

        // Sets network setting
        inputs = networkSettings.deepNetRecord.networkTopology[0];
        hidden = new int[networkSettings.deepNetRecord.networkTopology.Length-2];
        Array.Copy(networkSettings.deepNetRecord.networkTopology, 1, hidden, 0, networkSettings.deepNetRecord.networkTopology.Length-2);
        outputs = networkSettings.deepNetRecord.networkTopology[networkSettings.deepNetRecord.networkTopology.Length-1];

        mutationStrength = networkSettings.deepNetRecord.mutationStrength;
        mutationChance = networkSettings.deepNetRecord.mutationStrength;
        populationSize = networkSettings.deepNetRecord.populationSize;
        parentPopulationSize = networkSettings.deepNetRecord.parentPopulationSize;

        mutationReshuffleStrength = networkSettings.deepNetRecord.reshuffleMutationStrength;
        mutationReshuffleChance = networkSettings.deepNetRecord.reshuffleMutationChance;

        // Create a network based on the weights and biases from lauded network settings
        DeepNetMutate laudedNetwork = new DeepNetMutate(inputs, hidden, outputs);

        // Loads weights
        laudedNetwork.ihWeights = networkSettings.deepNetRecord.ihWeights;
        laudedNetwork.hhWeights = networkSettings.deepNetRecord.hhWeights;
        laudedNetwork.hoWeights = networkSettings.deepNetRecord.hoWeights;

        // Loads biases
        laudedNetwork.hBiases = networkSettings.deepNetRecord.hBiases;
        laudedNetwork.oBiases = networkSettings.deepNetRecord.oBiases;

        // Updates current weights and biases since they been set directly not using the setters
        laudedNetwork.UpdateCurrentWeightsArray();
        laudedNetwork.UpdateCurrentBiasesArray();

        // TETS START
        laudedNetwork.Dump(showWheight: true, showBiases: true);
        // TEST END
        
        if (trainingEnabled)
        {
            // If training enabled
            GeneratePopulation(populationSize, new TestResults[] { new TestResults { deepNet = laudedNetwork, wheight = laudedNetwork.getWeights(), biases = laudedNetwork.getBiases() } });
        }
        else
        {
            // If training is not enabled (Only a showcase)
            populationSize = 1;
            parentPopulationSize = 1;
            mutationChance = 0;
            mutationStrength = 0;
            mutationReshuffleChance = 0;
            mutationReshuffleStrength = 0;

            GeneratePopulation(populationSize, new TestResults[] { new TestResults { deepNet = laudedNetwork, wheight = laudedNetwork.getWeights(), biases = laudedNetwork.getBiases() } });
        }

    }

    public struct TestResults
    {
        public float fitness;
        public double[] wheight;
        public double[] biases;
        public DeepNetMutate deepNet;
    }
}
