using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Threading;
using System.Threading.Tasks;

public class OptionsMainUI : MonoBehaviour
{
    public Button saveButton;
    public Button saveAsButton;

    public GameObject optionsMainContainer;
    public Animator optionsMainAnimator;

    public GameObject optionsSaveContainer;
    public Animator optionsSaveAnimator;

    public GameObject optionsSavingContainer;
    public Animator optionsSavingAnimator;

    public GameObject savedSuccessfullyContainer;
    public Animator savedSuccessfullyAnimator;
    public PopupMessageUI savedSuccessfullyScript;

    public GameObject popupYesNoContainer;
    public Animator popupYesNoAnimator;
    public PopupYesNoUI popupYesNoUIScript;

    private Manager manager;

    // Start is called before the first frame update
    void Start()
    {
        // Finds manager
        manager = FindObjectOfType<Manager>();

        // Disables save button
        saveButton.interactable = false;
        saveAsButton.interactable = false;
    }

    private void Update()
    {
        if (FindObjectOfType<NetworkSettings>().deepNetRecord.generation+1 < FindObjectOfType<Manager>().generation)
        {
            saveButton.interactable = true;
            saveAsButton.interactable = true;
        }
    }

    public void HomeBtn()
    {
        // Asks the user whenever they want to go back
        popupYesNoContainer.SetActive(true);
        popupYesNoAnimator.SetTrigger("Show");
        popupYesNoUIScript.callBack = YesNoCallback;
    }

    public void YesNoCallback(PopupYesNoUI.Anwser anwser)
    {
        if (anwser == PopupYesNoUI.Anwser.Yes)
        {
            FindObjectOfType<SceneLoader>().LoadScene("Home");
        }
    }

    #region Save Button Functionalities

    public void SaveBtn()
    {
        // Deletes the old version
        DB_Controller.DeletDeepNet(FindObjectOfType<NetworkSettings>().name);
        // Saves the new version
        Save();
        // Shows saved successfully message
        savedSuccessfullyScript.callBack = () => { };
        savedSuccessfullyContainer.SetActive(true);
        savedSuccessfullyAnimator.SetTrigger("Show");
    }

    public void Save()
    {
        // Create a record to save
        DB_Controller.DeepNetRecord record = new DB_Controller.DeepNetRecord();

        record.name = FindObjectOfType<NetworkSettings>().name;
        record.networkTopology = manager.bestNet.deepNet.networkTopology;
        record.generation = manager.generation;
        record.populationSize = manager.populationSize;
        record.parentPopulationSize = manager.parentPopulationSize;
        record.mutationChance = manager.mutationChance;
        record.mutationStrength = manager.mutationStrength;
        record.reshuffleMutationChance = manager.mutationReshuffleChance;
        record.reshuffleMutationStrength = manager.mutationReshuffleStrength;

        record.ihWeights = manager.bestNet.deepNet.ihWeights;
        record.hhWeights = manager.bestNet.deepNet.hhWeights;
        record.hoWeights = manager.bestNet.deepNet.hoWeights;

        record.hBiases = manager.bestNet.deepNet.hBiases;
        record.oBiases = manager.bestNet.deepNet.oBiases;

        DB_Controller.SaveDeepNet(record);

        // Wait to show my amazing animation
        Thread.Sleep(2000);
    }

    #endregion

    public void SaveAsBtn()
    {
        //Debug.Log("Save as button pressed");

        optionsMainContainer.GetComponent<CanvasGroup>().interactable = false;
        optionsSaveContainer.SetActive(true);
        optionsSaveAnimator.SetTrigger("Show");

    }

    public void BackBtn()
    {
        //Debug.Log("Back button pressed");

        // Plays the hide animation and disables the panel/container
        optionsMainContainer.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Time.timeScale = manager.simulationTimeScale;
        optionsMainAnimator.SetTrigger("Hide");
        Invoke("BackBtnInvokeFunction", 0.5f*Time.timeScale);

    }

    public void BackBtnInvokeFunction()
    {
        optionsMainContainer.GetComponent<CanvasGroup>().blocksRaycasts = true;
        optionsMainContainer.SetActive(false);
    }
}
