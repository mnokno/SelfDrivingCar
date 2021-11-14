using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer : MonoBehaviour
{
    public Camera viewCamera;
    public ViewMode viewMode = ViewMode.Fixed;

    // Fixed variables
    public Vector3 fixedPos;
    public Quaternion fixedRot;

    // Free variables
    private bool aPressed;
    private bool dPressed;
    private bool wPressed;
    private bool sPressed;
    private bool qPressed;
    private bool ePressed;

    private float aVelocity;
    private float dVelocity;
    private float wVelocity;
    private float sVelocity;
    private float qVelocity;
    private float eVelocity;

    public float acceleration;
    public float angularAcceleration;
    public float maxAcceleration;
    public float maxAngularAcceleration;

    private Vector3 yAxis = new Vector3(0, 1, 0);
    private Vector3 xAxis = new Vector3(1, 0, 0);

    public Vector3 freePos;
    public Quaternion freeRot;

    // Follow variables
    public GameObject followObject;

    // Drop dawn
    public TMPro.TMP_Dropdown dropdawn;

    // Start is called before the first frame update
    void Start()
    {
        // Saves position of fixed camera
        fixedPos = viewCamera.transform.position;
        fixedRot = viewCamera.transform.rotation;

        // Sets initial position for the free position
        freePos = fixedPos;
        freeRot = fixedRot;
    }

    // Update is called once per frame
    void Update()
    {
        aPressed = Input.GetKey("a");
        dPressed = Input.GetKey("d");
        wPressed = Input.GetKey("w");
        sPressed = Input.GetKey("s");
        qPressed = Input.GetKey("q");
        ePressed = Input.GetKey("e");

        // Exits follow mode on escape pressed
        if (viewMode == ViewMode.Follow)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                viewMode = ViewMode.Fixed;
                dropdawn.value = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        // Updates velocities
        // a
        if (aPressed)
        {
            aVelocity += angularAcceleration;
            if (aVelocity >= maxAngularAcceleration)
            {
                aVelocity = maxAngularAcceleration;
            }
        }
        else
        {
            aVelocity = 0;
        }

        // d
        if (dPressed)
        {
            dVelocity += angularAcceleration;
            if (dVelocity >= maxAngularAcceleration)
            {
                dVelocity = maxAngularAcceleration;
            }
        }
        else
        {
            dVelocity = 0;
        }

        // w
        if (wPressed)
        {
            wVelocity += acceleration;
            if (wVelocity >= maxAcceleration)
            {
                wVelocity = maxAcceleration;
            }
        }
        else
        {
            wVelocity = 0;
        }

        // s
        if (sPressed)
        {
            sVelocity += acceleration;
            if (sVelocity >= maxAcceleration)
            {
                sVelocity = maxAcceleration;
            }
        }
        else
        {
            sVelocity = 0;
        }

        // q
        if (qPressed)
        {
            qVelocity += angularAcceleration;
            if (qVelocity >= maxAngularAcceleration)
            {
                qVelocity = maxAngularAcceleration;
            }
        }
        else
        {
            qVelocity = 0;
        }
        
        // e
        if (ePressed)
        {
            eVelocity += angularAcceleration;
            if (eVelocity >= maxAngularAcceleration)
            {
                eVelocity = maxAngularAcceleration;
            }
        }
        else
        {
            eVelocity = 0;
        }

        switch (viewMode) // Updates viewing mode
        {
            case ViewMode.Fixed:
                // Places view object in a fixed position
                this.transform.position = fixedPos;
                this.transform.rotation = fixedRot;
                // Snaps the camera to view object
                SnapCamera();
                // Removes follow object
                followObject = null;
                break;
            case ViewMode.Free:
                // Places view object in a free position
                this.transform.position = freePos;
                this.transform.rotation = freeRot;
                // Updates position
                this.transform.position += this.transform.forward * wVelocity * Time.deltaTime / Time.timeScale;
                this.transform.position -= this.transform.forward * sVelocity * Time.deltaTime / Time.timeScale;
                this.transform.Rotate(-xAxis * qVelocity * Time.deltaTime / Time.timeScale / 2);
                this.transform.Rotate(-xAxis * qVelocity * Time.deltaTime / Time.timeScale / 2);
                this.transform.Rotate(xAxis * eVelocity * Time.deltaTime / Time.timeScale / 2);
                this.transform.Rotate(-yAxis * aVelocity * Time.deltaTime / Time.timeScale, Space.World);
                this.transform.Rotate(yAxis * dVelocity * Time.deltaTime / Time.timeScale, Space.World);
                // Updates freePos and freeRot
                freePos = this.transform.position;
                freeRot = this.transform.rotation;
                // Snaps the camera to view object
                SnapCamera();
                // Removes follow object
                followObject = null;
                break;
            case ViewMode.Follow:
                if (followObject != null)
                {
                    // Places view object where the follow object is located
                    this.transform.position = followObject.transform.position;
                    this.transform.rotation = followObject.transform.rotation;
                    this.transform.Rotate(yAxis * 90, Space.World);
                    // Snaps the camera to view object
                    SnapCamera();
                }
                break;
        }
    }

    private void SnapCamera()
    {
        // Places camera where the view object is
        viewCamera.transform.position = this.transform.position;
        viewCamera.transform.rotation = this.transform.rotation;
    } 

    public enum ViewMode
    {
        Fixed, Free, Follow
    }
}
