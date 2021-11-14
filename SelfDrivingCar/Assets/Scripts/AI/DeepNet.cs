using System;
using UnityEngine;

public class DeepNet
{
    public static System.Random rnd = new System.Random();

    public int nInput;  // number input nodes
    public int[] nHidden;  // number hidden nodes, each layer
    public int nOutput;  // number output nodes
    public int nLayers;  // number hidden node layers
    public int[] networkTopology; // Combines nInput nHidden nOutput

    public double[] iNodes;  // input nodes
    public double[][] hNodes; // hidden nodes
    public double[] oNodes; // output nodes

    public double[][] ihWeights;  // input- 1st hidden
    public double[][][] hhWeights; // hidden-hidden
    public double[][] hoWeights;  // last hidden-output

    public double[][] hBiases;  // hidden node biases
    public double[] oBiases;  // output node biases

    private double[] cWeights; // current list of weight
    private double[] cBiases; // current list of biases

    // Constructor
    public DeepNet(int numInput, int[] numHidden, int numOutput)
    {
        this.nInput = numInput;
        this.nHidden = new int[numHidden.Length];
        for (int i = 0; i < numHidden.Length; ++i)
            this.nHidden[i] = numHidden[i];
        this.nOutput = numOutput;
        this.nLayers = numHidden.Length;

        this.iNodes = new double[numInput];
        this.hNodes = DeepNet.MakeJaggedMatrix(numHidden);
        this.oNodes = new double[numOutput];

        this.ihWeights = DeepNet.MakeMatrix(numInput, numHidden[0]);
        this.hhWeights = new double[numHidden.Length - 1][][];
        for (int i = 0; i < hhWeights.Length; i++)
        {
            this.hhWeights[i] = DeepNet.MakeMatrix(numHidden[i], numHidden[i + 1]);
        }
        this.hoWeights = DeepNet.MakeMatrix(numHidden[numHidden.Length - 1], numOutput);

        this.hBiases = DeepNet.MakeJaggedMatrix(numHidden);
        this.oBiases = new double[numOutput];

        InitializeWeights();
        InitializeBiases();
        InitializeNetworkTopology();
    }

    // Initialization
    private void InitializeWeights()
    {
        // Number of weights
        int nWeights = DeepNet.NumWeights(nInput, nHidden, nOutput)[0];

        // Generates random small number between 0.01 and 0.1
        double[] wts = new double[nWeights];
        for (int i = 0; i < nWeights; i++)
        {
            wts[i] = (rnd.Next(1, 100) * 0.001);
        }

        SetWeights(wts);
    }

    private void InitializeBiases()
    {
        // Number of weights
        int nBiases = DeepNet.NumWeights(nInput, nHidden, nOutput)[1];

        // Generates random small number between 0.01 and 0.1
        double[] bts = new double[nBiases];
        for (int i = 0; i < nBiases; i++)
        {
            bts[i] = (rnd.Next(1, 100) * 0.001);
        }

        SetBiases(bts);
    }

    private void InitializeNetworkTopology()
    {
        // Combines nInput nHidden nOutput
        networkTopology = new int[nHidden.Length + 2];

        networkTopology[0] = nInput;
        for (int i = 0; i < nHidden.Length; i++)
        {
            networkTopology[i + 1] = nHidden[i];
        }
        networkTopology[networkTopology.Length - 1] = nOutput;
    }

    // Computing outputs
    public double[] ComputeOutputs(double[] inputs)
    {
        // SETUP FOR COMPUTING OUTPUTS 

        // Sets input nodes to the passed inputs
        for (int i = 0; i < nInput; i++)
        {
            iNodes[i] = inputs[i];
        }

        // Sets hNodes and oNodes to 0
        for (int i = 0; i < hNodes.Length; i++)
        {
            for (int j = 0; j < hNodes[i].Length; j++)
            {
                hNodes[i][j] = 0;
            }
        }
        for (int i = 0; i < nOutput; i++)
        {
            oNodes[i] = 0;
        }

        // COMPUTING OUTPUTS 

        // Calculate value of the nodes from the first hidden layer
        for (int i = 0; i < hNodes[0].Length; i++)
        {
            // Sum of the products of the nodes and its weights
            for (int j = 0; j < nInput; j++)
            {
                hNodes[0][i] += ihWeights[j][i] * iNodes[j];
            }
            // Adds the bias
            hNodes[0][i] += hBiases[0][i];
            // Uses the activation function
            hNodes[0][i] = TanhProxy(hNodes[0][i]);

        }

        // Calculate value of the nodes form the rest of hidden layers
        for (int i = 1; i < nLayers; i++)
        {
            for (int j = 0; j < nHidden[i]; j++)
            {
                // Sum of the products of the nodes and its weights
                for (int k = 0; k < nHidden[i - 1]; k++)
                {
                    hNodes[i][j] += hhWeights[i - 1][k][j] * hNodes[i - 1][k];
                }
                // Adds the bias
                hNodes[i][j] += hBiases[i][j];
                // Uses the activation function
                hNodes[i][j] = TanhProxy(hNodes[i][j]);
            }
        }

        // Calculate value of the nodes from the output layer
        for (int i = 0; i < nOutput; i++)
        {
            // Sum of the products of the nodes and its weights
            for (int j = 0; j < nHidden[nLayers - 1]; j++)
            {
                oNodes[i] += hoWeights[j][i] * hNodes[nLayers - 1][j];
            }
            // Adds the bias
            oNodes[i] += oBiases[i];
        }

        // Applies softmax
        oNodes = Softmax(oNodes);

        // Returns final values of the nodes from the output layer
        return oNodes;
    }

    private static double TanhProxy(double x)
    {
        if (x > 20)
        {
            return 1;
        }
        else if (x < -20)
        {
            return -1;
        }
        else
        {
            return Math.Tanh(x);
        }
    }

    private static double[] Softmax(double[] oNodes)
    {
        // softmax(x) = e ^ x / (e ^ x + e ^ y + e ^ z)
        // softmax(y) = e ^ y / (e ^ x + e ^ y + e ^ z)
        // softmax(z) = e ^ z / (e ^ x + e ^ y + e ^ z)

        // Find the max value for the softmax max trick
        double max = 0;
        foreach (double value in oNodes)
        {
            if (value > max) max = value;
        }

        // Calculates denominator
        double denominator = 0;
        foreach (double value in oNodes)
        {
            denominator += Math.Exp(value - max);
        }

        // Calculates the resulting values
        double[] product = new double[oNodes.Length];
        for (int i = 0; i < product.Length; i++)
        {
            product[i] = Math.Exp(oNodes[i] - max) / denominator;
        }

        // Returns the product
        return product;
    }

    // Setting weights and biases
    public void SetWeights(double[] wts)
    {
        // Checks if correct number of wherewith has been passed
        int nw = NumWeights(nInput, nHidden, nOutput)[0];
        if (nw != wts.Length)
        {
            throw new Exception("Bad wts[] length in SetWeights()");
        }

        cWeights = wts;

        // Counter for the next weight
        int ptr = 0;

        // Sets weights for input to hidden
        for (int i = 0; i < nInput; i++)
        {
            for (int j = 0; j < nHidden[0]; j++)
            {
                ihWeights[i][j] = wts[ptr];
                ptr++;
            }
        }

        // Sets weights for hidden to hidden
        for (int i = 0; i < nLayers - 1; i++)
        {
            for (int j = 0; j < nHidden[i]; j++)
            {
                for (int h = 0; h < nHidden[i + 1]; h++)
                {
                    hhWeights[i][j][h] = wts[ptr];
                    ptr++;
                }
            }
        }

        // Sets weights for hidden to output
        for (int i = 0; i < hNodes[nLayers - 1].Length; i++)
        {
            for (int j = 0; j < nOutput; j++)
            {
                hoWeights[i][j] = wts[ptr];
                ptr++;
            }
        }
    }

    public void SetBiases(double[] bts)
    {
        // Checks if correct number of wherewith has been passed
        int nb = NumWeights(nInput, nHidden, nOutput)[1];
        if (nb != bts.Length)
        {
            throw new Exception("Bad bts[] length in SetWeights()");
        }

        cBiases = bts;

        // Counter for the next weight
        int ptr = 0;

        // Hidden biases
        for (int i = 0; i < nLayers; i++)
        {
            for (int j = 0; j < nHidden[i]; j++)
            {
                hBiases[i][j] = bts[ptr];
                ptr++;
            }
        }

        // Output biases
        for (int i = 0; i < nOutput; i++)
        {
            oBiases[i] = bts[ptr];
            ptr++;
        }

    }

    // Manual weights and biases activation functions
    public void UpdateCurrentWeightsArray()
    {
        // Finds total number of weights
        cWeights = new double[NumWeights(nInput, nHidden, nOutput)[0]];

        // Adds all weight to the cWeights array
        int currentIndex = 0;

        // Adds each weight from input to hidden to the cWeights array
        for (int i = 0; i < nInput; i++) // For each node in input layer
        {
            for (int j = 0; j < nHidden[0]; j++) // For each node in first hidden layer
            {
                cWeights[currentIndex] = ihWeights[i][j];
                currentIndex++;
            }
        }

        // Adds each weight from hidden to hidden to the cWeights array
        for (int i = 0; i < nLayers-1; i++) // For each hidden layer but the lest one
        {
            for (int j = 0; j < nHidden[i]; j++) // For each node in the hidden layer
            {
                for (int k = 0; k < nHidden[i+1]; k++) // For each node in the next hidden layer
                {
                    cWeights[currentIndex] = hhWeights[i][j][k];
                    currentIndex++;
                }
            }                
        }

        // Adds each weight from hidden to output to the cWeights array
        for (int i = 0; i < nHidden[nLayers-1]; i++) // For each node in the last hidden layer
        {
            for (int j = 0; j < nOutput; j++) // For each node in the output layer
            {
                cWeights[currentIndex] = hoWeights[i][j];
                currentIndex++;
            }
        }

        // TEST SECTION START
        if (currentIndex != NumWeights(nInput, nHidden, nOutput)[0])
        {
            Debug.LogError($"public void UpdateCurrentWeightsArray curretnIndex = {currentIndex}, but should be {NumWeights(nInput, nHidden, nOutput)[0]}!!!");
        }
        // TEST SECTION END
    }

    public void UpdateCurrentBiasesArray()
    {
        // Finds total number of biases
        cBiases = new double[NumWeights(nInput, nHidden, nOutput)[1]];

        // Adds all biases to the cBisaes array
        int currentIndex = 0;

        // Adds all hidden biases to the cBisses array
        for (int i = 0; i < nLayers; i++) // For each hidden layer
        {
            for (int j = 0; j < nHidden[i]; j++) // For each node in the layer
            {
                cBiases[currentIndex] = hBiases[i][j];
                currentIndex++;
            }
        }

        // Adds all output biases to the cBiases array
        for (int i = 0; i < nOutput; i++) // For each output node
        {
            cBiases[currentIndex] = oBiases[i];
            currentIndex++;
        }

        // TEST SECTION START
        if (currentIndex != NumWeights(nInput, nHidden, nOutput)[1])
        {
            Debug.LogError($"public void UpdateCurrentBiasesArray: curretnIndex = {currentIndex}, but should be {NumWeights(nInput, nHidden, nOutput)[1]}!!!");
        }
        // TEST SECTION END
    }

    // Support functions
    public static int[] NumWeights(int numInput, int[] numHidden, int numOutput)
    {
        // calculates total number of weights 
        int ihWts = numInput * numHidden[0];
        int hhWts = 0;
        for (int i = 0; i < numHidden.Length - 1; i++)
        {
            int rows = numHidden[i];
            int cols = numHidden[i + 1];
            hhWts += rows * cols;
        }
        int hoWts = numHidden[numHidden.Length - 1] * numOutput;
        int totWts = ihWts + hhWts + hoWts;

        // calculates total number of biases;
        int totBes = 0;
        for (int i = 0; i < numHidden.Length; i++)
        {
            totBes += numHidden[i];
        }
        totBes += numOutput;

        // returns the result in an array [total weights, total biases];
        return new int[2] { totWts, totBes };
    }

    public static double[][] MakeJaggedMatrix(int[] columns)
    {
        int rows = columns.Length;
        double[][] product = new double[rows][];
        for (int i = 0; i < rows; i++)
        {
            product[i] = new double[columns[i]];
        }
        return product;
    }

    public static double[][] MakeMatrix(int rows, int columns)
    {
        double[][] product = new double[rows][];
        for (int i = 0; i < rows; i++)
        {
            product[i] = new double[columns];
        }
        return product;
    }

    public void Dump(bool showNodes = false, bool showWheight = false, bool showBiases = false, string format = "F4")
    {
        string dumpLog = "\n";

        if (showNodes)
        {
            // Shows values of the nodes from input layer
            for (int i = 0; i < nInput; i++)
            {
                dumpLog += ($"input, node [{i}] = {iNodes[i].ToString(format)}");
                dumpLog += ("\n");
            }

            dumpLog += ("\n");

            // Shows values of the nodes from each hidden layer
            for (int i = 0; i < nLayers; i++)
            {
                for (int j = 0; j < nHidden[i]; j++)
                {
                    dumpLog += ($"hidden layer {i}, node [{j}] = {hNodes[i][j].ToString(format)}");
                    dumpLog += ("\n");
                }
                dumpLog += ("\n");
            }

            // Shows values of the nodes from output layer
            for (int i = 0; i < nOutput; i++)
            {
                dumpLog += ($"output, node [{i}] = {oNodes[i].ToString(format)}");
                dumpLog += ("\n");
            }
        }

        dumpLog += ("\n");

        if (showWheight)
        {
            // Shows weights of input to first hidden layer
            for (int i = 0; i < nInput; i++)
            {
                for (int j = 0; j < nHidden[0]; j++)
                {
                    dumpLog += ($"input-hidden, weight [{i}][{j}] = {ihWeights[i][j].ToString(format)}");
                    dumpLog += ("\n");
                }
            }

            dumpLog += ("\n");

            // Shows weights of hidden layers
            for (int i = 0; i < nLayers - 1; i++)
            {
                for (int j = 0; j < nHidden[i]; j++)
                {
                    for (int k = 0; k < nHidden[i + 1]; k++)
                    {
                        dumpLog += ($"hidden-hidden weight, layer [{i}] to layer [{i + 1}], node [{j}] to node [{k}], = {hhWeights[i][j][k].ToString(format)}");
                        dumpLog += ("\n");
                    }
                }
                dumpLog += ("\n");
            }

            // Shows weight of the hidden to output layer
            for (int i = 0; i < nHidden[nLayers - 1]; i++)
            {
                for (int j = 0; j < nOutput; j++)
                {
                    dumpLog += ($"hidden-output, weight [{i}][{j}] = {hoWeights[i][j].ToString(format)}");
                    dumpLog += ("\n");
                }
            }

        }

        dumpLog += ("\n");

        if (showBiases)
        {
            // Show biases of each hidden layer
            for (int i = 0; i < nLayers; i++)
            {
                for (int j = 0; j < nHidden[i]; j++)
                {
                    dumpLog += ($"hidden layer {i}, bias [{j}] = {hBiases[i][j].ToString(format)}");
                    dumpLog += ("\n");
                }
                dumpLog += ("\n");
            }

            // Shows biases of output layer 
            for (int i = 0; i < nOutput; i++)
            {
                dumpLog += ($"output, bias [{i}] = {oBiases[i].ToString(format)}");
                dumpLog += ("\n");
            }
        }

        Debug.Log(dumpLog);
    }

    // Getters
    public double[] getWeights()
    {
        return this.cWeights;
    }

    public double[] getBiases()
    {
        return this.cBiases;
    }
}
