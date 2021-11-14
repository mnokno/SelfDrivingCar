using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;


public class OptionsSaveUI : MonoBehaviour
{

    public GameObject optionsMainContainer;

    public GameObject optionsSaveContainer;
    public Animator optionsSaveAnimator;

    public GameObject optionsSavingContainer;
    public Animator optionsSavingAnimator;

    public GameObject popupMessageContainer;
    public Animator popupMessageAnimator;
    public PopupMessageUI popupMessageUIScript;

    public GameObject popupYesNoContainer;
    public Animator popupYesNoAnimator;
    public PopupYesNoUI popupYesNoUIScript;

    public TMPro.TMP_InputField inputField;

    private Manager manager;
    private bool saved = false;

    // Start is called before the first frame update
    void Start()
    {
        // Finds manager
        manager = FindObjectOfType<Manager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackBtn()
    {
        // Plays the hide animation and disables the panel/container 
        optionsMainContainer.GetComponent<CanvasGroup>().interactable = true;
        StartCoroutine("BackBtnInvokeCoroutine");
    }

    IEnumerator BackBtnInvokeCoroutine()
    {
        // Hides the save panel
        optionsSaveContainer.GetComponent<CanvasGroup>().blocksRaycasts = false;
        optionsSaveAnimator.SetTrigger("Hide");
        yield return new WaitForSecondsRealtime(0.5f);
        optionsSaveContainer.GetComponent<CanvasGroup>().blocksRaycasts = true;
        optionsSaveContainer.SetActive(false);
    }

    public void SaveBtn()
    {

        // Check if a nett with that name already exists
        if (DB_Controller.DoesContainName(inputField.text))
        {
            popupYesNoUIScript.callBack = YesNoCallback;
            popupYesNoContainer.SetActive(true);
            popupYesNoAnimator.SetTrigger("Show");
        }
        else
        {
            // Shows saving screen
            optionsSavingContainer.SetActive(true);
            optionsSavingAnimator.SetTrigger("Show");

            // Saves the record
            StartCoroutine(nameof(CheckIsSaved));
            Task t = new Task(Save);
            t.Start();
        }
    }

    public void YesNoCallback(PopupYesNoUI.Anwser anwser)
    {
        if (anwser == PopupYesNoUI.Anwser.Yes)
        {
            // Deletes the old record
            DB_Controller.DeletDeepNet(inputField.text);

            // Shows saving screen
            optionsSavingContainer.SetActive(true);
            optionsSavingAnimator.SetTrigger("Show");

            // Saves the record
            StartCoroutine(nameof(CheckIsSaved));
            Task t = new Task(Save);
            t.Start();
        }
    }

    public void Save()
    {
        // Create a record to save
        DB_Controller.DeepNetRecord record = new DB_Controller.DeepNetRecord();

        record.name = inputField.text;
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

        // Calls the callback function
        saved = true;
    }

    public void SaveCallback()
    {
        // Hides saving screen
        StartCoroutine(nameof(HideOptionsSaving));

        // HIdes the save panel
        BackBtn();

        // Show saved successfully message
        popupMessageUIScript.callBack = () => { };
        popupMessageContainer.SetActive(true);
        popupMessageAnimator.SetTrigger("Show");
    }

    IEnumerator HideOptionsSaving()
    {
        // Hides the saving panel
        optionsSavingAnimator.SetTrigger("Hide");
        yield return new WaitForSecondsRealtime(0.25f);
        optionsSavingContainer.SetActive(false);
    }

    IEnumerator CheckIsSaved()
    {
        while (!saved)
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }

        if (saved)
        {
            saved = false;
            SaveCallback();
        }
    }
}
