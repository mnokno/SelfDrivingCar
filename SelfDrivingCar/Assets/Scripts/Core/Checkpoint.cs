using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
    // Updates the fitness of the player
    collider.gameObject.GetComponent<PlayerManager>().fitness += 1;
    }
}
