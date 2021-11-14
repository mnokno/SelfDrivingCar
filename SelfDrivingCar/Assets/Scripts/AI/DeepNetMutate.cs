using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepNetMutate : DeepNet
{
    public DeepNetMutate(int numInput, int[] numHidden, int numOutput) : base(numInput, numHidden, numOutput)
    {

    }

    public void Mutate(float mutationChance, float mutationStrength)
    {
        // Gets weights and biases
        double[] weights = this.getWeights();
        double[] biases = this.getBiases();

        // Debug section start
        //Debug.Log($"mutationChance = {mutationChance}, mutationStrength = {mutationStrength}");
        // Debug section end

        // Mutates weights
        for (int i = 0; i < weights.Length; i++)
        {
            if (UnityEngine.Random.value <= mutationChance)
            {
                weights[i] += UnityEngine.Random.Range(-mutationStrength, mutationStrength);
            }
        }

        // Mutates biases
        for (int i = 0; i < biases.Length; i++)
        {
            if (UnityEngine.Random.value <= mutationChance)
            {
                biases[i] += UnityEngine.Random.Range(-mutationStrength, mutationStrength);
            }
        }

        // Updates mutated weights and biases
        this.SetWeights(weights);
        this.SetBiases(biases);
    }
}
