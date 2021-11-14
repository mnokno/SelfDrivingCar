using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerManager playerManager;
    public bool humanControlled = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Track")
        {
            playerManager.collided = true;

            if (!humanControlled)
            {
                playerManager.EndTest();
            }
        }
    }
}
