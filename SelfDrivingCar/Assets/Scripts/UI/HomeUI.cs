using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame 
    void Update()
    {
        
    }

    public void NewBtn()
    {
        // Loads Create scene
        FindObjectOfType<SceneLoader>().LoadScene("Create");
    }

    public void LoadBtn()
    {
        // Loads Load scene
        FindObjectOfType<SceneLoader>().LoadScene("Load");
    }

    public void QuitBtn()
    {
        // Quits the application
        Application.Quit();
    }

    public void ManagerBtn()
    {
        // Loads Manager scene
        FindObjectOfType<SceneLoader>().LoadScene("Manager");
    }
}
