using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSettings : MonoBehaviour
{

    public bool train;
    public string trackName;
    public DB_Controller.DeepNetRecord deepNetRecord;

    public static NetworkSettings instance;

    // Awake is called before start
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
