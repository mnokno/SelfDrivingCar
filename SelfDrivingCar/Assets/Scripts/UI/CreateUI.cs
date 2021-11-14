using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CreateUI : MonoBehaviour
{
    public Animator createAnimator;

    // Network parameters variables
    public GameObject content;
    public Button createButton;

    public TMP_InputField networkName;
    public TMP_InputField inputNodes;
    public TMP_InputField hiddenLayers;
    public TMP_InputField outputNodes;
    public TMP_InputField populationSize;
    public TMP_InputField parentPopulationSize;
    public TMP_InputField mutationChance;
    public TMP_InputField mutationStrength;
    public TMP_InputField geneticReshuffleChance;
    public TMP_InputField geneticReshuffleStrength;

    public GameObject networkParameterSlot;
    private GameObject[] activeNetworkParameterSlots = new GameObject[0];

    // Track selection variables
    public GameObject selectButtonTrack;
    private GameObject currentlySelectedTrack;

    // Pop-up
    // public GameObject popupYesNoContainer;
    // public GameObject popupYesNoScript;

    // Start is called before the first frame update
    void Start()
    {
        content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(content.GetComponent<RectTransform>().rect.width - 20, 50);
    }

    public void HiddenLayersOnValueChanged(string newValue)
    {
        // Check if value (string) is an int
        if (int.TryParse(newValue, out int value))
        {
            // Updates the number of hidden layers
            if (value == activeNetworkParameterSlots.Length) // No change is needed
            {
            }
            else if (value > activeNetworkParameterSlots.Length) // Adds hidden layers
            {
                GameObject[] temp = new GameObject[value];
                Array.Copy(activeNetworkParameterSlots, temp, activeNetworkParameterSlots.Length);

                for (int i = activeNetworkParameterSlots.Length; i < value; i++)
                {
                    // Create instance of a network parameter slot
                    GameObject newSlot = Instantiate(networkParameterSlot);
                    // Sets Parent
                    newSlot.transform.SetParent(content.transform);
                    // Sets Scale
                    newSlot.transform.localScale = new Vector3(1, 1, 1);
                    // Sets index 
                    newSlot.transform.SetSiblingIndex(i + 3);
                    // Sets Content (text)
                    newSlot.GetComponentInChildren<TextMeshProUGUI>().text = $" Nodes in Hidden Layer {i+1}:";
                    // Sets on edit end trigger
                    newSlot.GetComponentInChildren<TMP_InputField>().onEndEdit.AddListener(Validate);


                    // Adds the new slot to temp array
                    temp[i] = newSlot;
                }

                // Updates activeNetworkParameterSlots
                activeNetworkParameterSlots = temp;
            }
            else // Removes hidden layers
            {
                int slotsToDelete = activeNetworkParameterSlots.Length - value;
                for (int i = 0; i < slotsToDelete; i++)
                {
                    // Removes a slot
                    Destroy(activeNetworkParameterSlots[activeNetworkParameterSlots.Length - i - 1].gameObject);
                    
                }

                // Updates activeNetworkParameterSlots
                GameObject[] temp = new GameObject[activeNetworkParameterSlots.Length - slotsToDelete];
                Array.Copy(activeNetworkParameterSlots, temp, activeNetworkParameterSlots.Length - slotsToDelete);
                activeNetworkParameterSlots = temp;
            }
        }
    }

    public void GoHomeBtn()
    {
        // Loads Home scene
        FindObjectOfType<SceneLoader>().LoadScene("Home");
    }

    public void Validate(string text = "")
    {
        // Checks the parameters
        if (
            networkName.text != "" && !DB_Controller.DoesContainName(networkName.text) && // network name field validation
            hiddenLayers.text != "" && int.Parse(hiddenLayers.text) > 0 && // hidden layers filed verification
            populationSize.text != "" && int.Parse(populationSize.text) > 0 && // population size field verification
            parentPopulationSize.text != "" && int.Parse(parentPopulationSize.text) > 0 && int.Parse(parentPopulationSize.text) < int.Parse(populationSize.text) && // parent population size field verification
            mutationChance.text != "" && float.Parse(mutationChance.text) > 0 && float.Parse(mutationChance.text) <= 1 && // mutation chance field verification
            mutationStrength.text != "" && float.Parse(mutationStrength.text) > 0 && // mutation strength field verification
            geneticReshuffleChance.text != "" && float.Parse(geneticReshuffleChance.text) > 0 && float.Parse(geneticReshuffleChance.text) <= 1 && // genetic reshuffle chance field verification
            geneticReshuffleStrength.text != "" && float.Parse(geneticReshuffleStrength.text) > 0 // genetic reshuffle strength field verification
            )
        {
            bool valid = true;
            foreach (GameObject go in activeNetworkParameterSlots)
            {
                // Verifies all fields from activeNetworkParameterSlots
                string activeNetworkParameterSlotsValue = go.GetComponentInChildren<TMP_InputField>().text;
                if (activeNetworkParameterSlotsValue == "" || int.Parse(activeNetworkParameterSlotsValue) <= 0)
                {
                    valid = false;
                }
            }

            if (valid)
            {
                createButton.interactable = true;
            }
            else
            {
                createButton.interactable = false;
            }
        }
        else
        {
            createButton.interactable = false;
        }
    }

    public void CreateBtn()
    {
        // Create the network
        int inputs = int.Parse(inputNodes.text);
        int[] hidden = new int[int.Parse(hiddenLayers.text)];
        for (int i = 0; i < hidden.Length; i++)
        {
            hidden[i] = int.Parse(activeNetworkParameterSlots[i].GetComponentInChildren<TMP_InputField>().text);
        }
        int outputs = int.Parse(outputNodes.text);
        DeepNet deepNet = new DeepNet(inputs, hidden, outputs);

        // Constructs DeepNetRecord
        DB_Controller.DeepNetRecord record = new DB_Controller.DeepNetRecord();
        record.name = networkName.text;
        record.networkTopology = deepNet.networkTopology;
        record.generation = 0;
        record.populationSize = int.Parse(populationSize.text);
        record.parentPopulationSize = int.Parse(parentPopulationSize.text);

        record.mutationChance = float.Parse(mutationChance.text);
        record.mutationStrength = float.Parse(mutationStrength.text);
        record.reshuffleMutationChance = float.Parse(geneticReshuffleChance.text);
        record.reshuffleMutationStrength = float.Parse(geneticReshuffleStrength.text);

        record.ihWeights = deepNet.ihWeights;
        record.hhWeights = deepNet.hhWeights;
        record.hoWeights = deepNet.hoWeights;

        record.hBiases = deepNet.hBiases;
        record.oBiases = deepNet.oBiases;

        // Saves the network to the data base
        DB_Controller.SaveDeepNet(record);

        // Sets the network name in network settings
        FindObjectOfType<NetworkSettings>().name = record.name;

        // Transitions from network parameters to track selection
        createAnimator.SetTrigger("CreateToNetwork");
    }

    public void LoadBtn()
    {
        // Finds network settings
        NetworkSettings networkSettings = FindObjectOfType<NetworkSettings>();
        // Enables training
        networkSettings.train = true;
        // Sets track name
        networkSettings.trackName = currentlySelectedTrack.name;
        // Loads the network
        networkSettings.deepNetRecord = DB_Controller.GetDeepNet(networkSettings.name);

        // Loads the correct scene
        FindObjectOfType<SceneLoader>().LoadScene(networkSettings.trackName);
    }

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
}
