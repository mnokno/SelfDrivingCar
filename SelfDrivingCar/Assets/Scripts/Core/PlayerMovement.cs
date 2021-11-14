using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public PlayerManager playerManager;
    public bool humanControlled = false;
    public float speed = 5;
    public int yRotation = 0;

    private Vector3 forward = new Vector3(1, 0, 0);
    private Vector3 rotationRight = new Vector3(0, 120, 0);
    private Vector3 rotationLeft = new Vector3(0, -120, 0);

    private Direction dir = Direction.Straight;

    public enum Direction
    {
        Left,
        Right,
        Straight
    }

    // Start is called before the first frame update
    void Start()
    {
        // rotate the forward direction by the current rotation
        forward = Quaternion.Euler(new Vector3(0, yRotation, 0)) * forward;
    }

    // Update is called once per frame
    void Update()
    {
        // Get Player input
        if (humanControlled)
        {
            if (Input.GetKey("a"))
            {
                dir = Direction.Left;
            }
            else if (Input.GetKey("d"))
            {
                dir = Direction.Right;
            }
            else
            {
                dir = Direction.Straight;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!playerManager.collided)
        {
            if (humanControlled)
            {
                //playerManager.distance += (forward * speed * Time.deltaTime).magnitude;
                transform.localPosition += forward * speed * Time.deltaTime;
                Turn(dir);
            }
            else
            {
                //playerManager.distance += (forward * speed * Time.deltaTime).magnitude;
                transform.localPosition += forward * speed * Time.deltaTime;
                Turn(playerManager.GetDirection());
            }
        }
    }

    public void Turn(Direction direction)
    {
        if (Direction.Right == direction)
        {
            transform.Rotate(rotationRight * Time.deltaTime, Space.World);
            forward = Quaternion.Euler(rotationRight * Time.deltaTime) * forward;
        }
        else if (Direction.Left == direction)
        {
            transform.Rotate(rotationLeft * Time.deltaTime, Space.World);
            forward = Quaternion.Euler(rotationLeft * Time.deltaTime) * forward;
        }
    }
}
