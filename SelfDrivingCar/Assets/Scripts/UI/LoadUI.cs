using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadUI : MonoBehaviour
{
    public Animator loadAnimator;

    // Track selection variables
    public GameObject selectButtonTrack;
    private GameObject currentlySelectedTrack;

    // Network selection variables
    public GameObject buttonPrefab;
    public GameObject buttonContainer;
    public GameObject selectButtonNetwork;
    private GameObject currentlySelectedNetwork;

    // Mode selection variables
    public GameObject toggleContinueTraining;
    public GameObject toggleShowcase;

    // Start is called before the first frame update
    void Start()
    {
        // Sets width of the gird layout
        buttonContainer.GetComponent<GridLayoutGroup>().cellSize = new Vector2(buttonContainer.GetComponent<RectTransform>().rect.width, 35);

        // Populates the list box (button container) with networks
        foreach (string name in DB_Controller.GetAllNetworkNames())
        {
            // Creates instance of button prefab
            GameObject currentButton = Instantiate(buttonPrefab);
            // Sets name for the object
            currentButton.name = name;
            // Disables animator
            currentButton.GetComponent<Animator>().enabled = false;
            // Sets parent for the object
            currentButton.transform.SetParent(buttonContainer.transform);
            // Sets text on the button
            currentButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;
            // Sets color
            currentButton.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f);
            // Sets transition
            currentButton.GetComponent<Button>().transition = Selectable.Transition.None;
            // Adds on click event
            currentButton.GetComponent<Button>().onClick.AddListener(() => SelectNetwork(currentButton));
            // Sets scale
            currentButton.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    //
    // Track Selection Section Start
    //

    public void SelectTrack(GameObject button)
    {
        if (currentlySelectedTrack == button) // De-Selects track with passed trackName
        {
            // Changes color of currently selected track to deselected (white)
            button.GetComponent<Image>().color = new Color(0.85f, 0.85f, 0.85f);
            // Removes the trackName to network setting
            FindObjectOfType<NetworkSettings>().trackName = null;
            // Disables select button
            selectButtonTrack.GetComponent<Button>().interactable = false;
            // Sets currently selected track to null
            currentlySelectedTrack = null;
        }
        else // Selects track with passed trackName
        {
            if (currentlySelectedTrack != null) // De-selects previously selected track
            {
                currentlySelectedTrack.GetComponent<Image>().color = new Color(0.85f, 0.85f, 0.85f);
            }

            // Saves the track name as currently selected track
            currentlySelectedTrack = button;
            // Changes button color to selected
            button.GetComponent<Image>().color = new Color(1, 1, 1);           
            // Enables select button
            selectButtonTrack.GetComponent<Button>().interactable = true;
        }
    }

    public void GoBackFromTrackSelection()
    {
        // Loads home scene
        FindObjectOfType<SceneLoader>().LoadScene("Home");
    }

    public void SelectFromTrackSelection()
    {
        // Adds the trackName to network setting
        FindObjectOfType<NetworkSettings>().trackName = currentlySelectedTrack.name;
        // Transitions from track selection to network selection
        loadAnimator.SetTrigger("TrackToNetwork");
    }

    //
    // Track Selection Section End
    //

    //
    // Network Selection Section Start
    //

    public void SelectNetwork(GameObject button)
    {
        if (currentlySelectedNetwork == button) // De-Selects network with passed networkName
        {
            // Changes color of currently selected network to deselected (white)
            button.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f);
            // Updates currently selected track to null
            currentlySelectedNetwork = null;
            // Disables select button
            selectButtonNetwork.GetComponent<Button>().interactable = false;
            // Sets currently selected network to null
            currentlySelectedNetwork = null;
        }
        else // Selects track with passed networkName
        {
            if (currentlySelectedNetwork != null) // De-selects previously selected network
            {
                currentlySelectedNetwork.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f);
            }

            // Saves the track name as currently selected track
            currentlySelectedNetwork = button;
            // Changes button color to selected
            button.GetComponent<Image>().color = new Color(1, 1, 1);
            // Enables select button
            selectButtonNetwork.GetComponent<Button>().interactable = true;
        }
    }

    public void GoBackFromNetworkSelection()
    {
        // Transition from network selection to track selection
        loadAnimator.SetTrigger("NetworkToTrack");
    }

    public void SelectFromNetworkSelection()
    {
        // Adds selected network to network settings
        FindObjectOfType<NetworkSettings>().deepNetRecord = DB_Controller.GetDeepNet(currentlySelectedNetwork.name);
        // Transition from network selection to mode selection
        loadAnimator.SetTrigger("NetworkToMode");
    }

    //
    // Network Selection Section End
    //

    // 
    // Mode Selection Section Start
    // 

    public void ToggleContinueTraining(bool value)
    {
        toggleShowcase.GetComponent<Toggle>().isOn = !value;
    }

    public void ToggleShowcase(bool value)
    {
        toggleContinueTraining.GetComponent<Toggle>().isOn = !value;
    }

    public void GoBackFromModeSelection()
    {
        // Transitions from mode selection to network selection
        loadAnimator.SetTrigger("ModeToNetwork");
    }

    public void LoadFromModeSelection()
    {
        // Updates network settings
        FindObjectOfType<NetworkSettings>().train = toggleContinueTraining.GetComponent<Toggle>().isOn;
        // Loads the selected track
        FindObjectOfType<SceneLoader>().LoadScene(currentlySelectedTrack.name);
    }

    // 
    // Mode Selection Section End
    //
}
