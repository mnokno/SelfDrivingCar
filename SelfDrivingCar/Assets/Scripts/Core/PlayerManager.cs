using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool collided = false;
    public float fitness = 0;
    public DeepNetMutate deepNet;
    private int index;

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void InitDeepNet(int numInput, int[] numHidden, int numOutput, double[] wts, double[] bts)
    {
        deepNet = new DeepNetMutate(numInput, numHidden, numOutput);
        deepNet.SetWeights(wts);
        deepNet.SetBiases(bts);
    }

    public void EndTest()
    {
        // Tells the manager result of the test
        Manager manager = FindObjectOfType<Manager>();
        manager.EndTest(index, fitness, deepNet.getWeights(), deepNet.getBiases(), deepNet);
    }

    public PlayerMovement.Direction GetDirection()
    {
        double[] res = deepNet.ComputeOutputs(GetDistances());

        if (res[0] > res[1])
        {
            return PlayerMovement.Direction.Right;
        }
        else
        {
            return PlayerMovement.Direction.Left;
        }
    }

    public double[] GetDistances()
    {
        double[] distnaces = new double[5];

        for (int i = 0; i < 5; i++)
        {
            Vector3 rayCastVector = Quaternion.AngleAxis(-45 * i - 180, new Vector3(0, 1, 0)) * transform.forward;

            RaycastHit hit;
            Ray ray = new Ray(transform.position, rayCastVector);

            int layerMask = 1 << 6;
            layerMask = ~layerMask;

            if (Physics.Raycast(ray, out hit, 1000, layerMask))
            {
                distnaces[i] = hit.distance;
                Debug.DrawRay(transform.position, rayCastVector * hit.distance, Color.green);
            }
            else
            {
                distnaces[i] = 1000;
            }
        }
        return distnaces;
    }

    public void Mutate(float mutationChance, float mutationStrength)
    {
        deepNet.Mutate(mutationChance, mutationStrength);
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (FindObjectOfType<Viewer>().viewMode == Viewer.ViewMode.Follow)
            {
                FindObjectOfType<Viewer>().followObject = this.gameObject;
            }
        }
    }
}
