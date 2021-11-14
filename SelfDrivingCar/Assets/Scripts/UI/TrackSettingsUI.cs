using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackSettingsUI : MonoBehaviour
{
    public Toggle genNextTog;
    public Toggle genReshTog;
    public Animator trackSettingsAnimator;

    public GameObject optionsMainContainer;
    public Animator optionsMainAnimator;

    private Manager manager;

    private void Start()
    {
        // Finds manager
        manager = FindObjectOfType<Manager>();

        // Start coroutines
        StartCoroutine("UpdateToggles");

        // Disables gemNext since auto gen is enabled by default
    }

    public IEnumerator UpdateToggles()
    {
        while (true)
        {
            // Check if reshuffle has happened
            genNextTog.isOn = manager.generateNext;

            // Checks if the next generation is generated
            genReshTog.isOn = manager.reshuffleNextGeneration;

            yield return new WaitForSeconds(0.05f);
        }
    }

    public void ShowBtn()
    {
        //Debug.Log("Show button pressed");

        // Shows the track settings
        trackSettingsAnimator.SetTrigger("Show");
    }

    public void HideBtn()
    {
        //Debug.Log("Hide button pressed");

        // Hides the track settings
        trackSettingsAnimator.SetTrigger("Hide");
    }

    public void OptionsBtn()
    {
        // Debug.Log("Options button pressed");

        // Show and enables options 
        Time.timeScale = 0;
        optionsMainContainer.SetActive(true);
        optionsMainAnimator.SetTrigger("Show");
    }

    public void AutoGenerateTog(bool currentState)
    {
        //Debug.Log($"Auto Gen toggled, state: {currentState}");

        // Updates the value in manager
        manager.autoStartNextGen = currentState;
    }

    public void GenerateNextTog(bool currentState)
    {
        //Debug.Log($"Gen Next toggled, state: {currentState}");

        // Updates the value in manager
        manager.generateNext = currentState;
    }

    public void GeneticReshuffleTog(bool currentState)
    {
        //Debug.Log($"Gen Resh toggled, state: {currentState}");

        // Updates the value in manager
        manager.reshuffleNextGeneration = currentState;
    }

    public void GameSpeedSld(float currentValue)
    {
        //Debug.Log($"Game Speed Changed to {currentValue}");

        // Update the time scale
        manager.simulationTimeScale = currentValue;
        Time.timeScale = currentValue;
    }

    public void UpdateView(int newModeIndex)
    {
        switch (newModeIndex)
        {
            case 0: // Fixed View
                FindObjectOfType<Viewer>().viewMode = Viewer.ViewMode.Fixed;
                break;
            case 1: // Free View
                FindObjectOfType<Viewer>().viewMode = Viewer.ViewMode.Free;
                break;
            case 2: // Follow View
                FindObjectOfType<Viewer>().viewMode = Viewer.ViewMode.Follow;
                break;
        }
    }
}
