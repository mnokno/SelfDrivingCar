using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    public GameObject content;
    public GameObject buttonPrefab;
    public GameObject deleteButtonNetwork;
    private GameObject currentlySelectedNetwork;

    // Pop-up
    public GameObject popupYesNoContainer;
    public PopupYesNoUI popupYesNoScript;

    // Start is called before the first frame update
    void Start()
    {
        // Sets width of the gird layout
        content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(content.GetComponent<RectTransform>().rect.width, 35);

        // Populates content with network records
        Populate();
    }

    public void Populate()
    {
        // Populates the list box (content) with networks
        foreach (string name in DB_Controller.GetAllNetworkNames())
        {
            // Creates instance of button prefab
            GameObject currentButton = Instantiate(buttonPrefab);
            // Sets name for the object
            currentButton.name = name;
            // Disables animator
            currentButton.GetComponent<Animator>().enabled = false;
            // Sets parent for the object
            currentButton.transform.SetParent(content.transform);
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

    public void SelectNetwork(GameObject button)
    {
        if (currentlySelectedNetwork == button) // De-Selects network with passed networkName
        {
            // Changes color of currently selected network to deselected (white)
            button.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f);
            // Updates currently selected track to null
            currentlySelectedNetwork = null;
            // Disables delete button
            deleteButtonNetwork.GetComponent<Button>().interactable = false;
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
            // Enables delete button
            deleteButtonNetwork.GetComponent<Button>().interactable = true;
        }
    }

    public void DeleteButton()
    {
        // Deletes currently selected record

        // Sets Callback function
        popupYesNoScript.callBack = Callback;

        // Ask for confirmation
        popupYesNoContainer.SetActive(true);
        popupYesNoContainer.GetComponent<Animator>().SetTrigger("Show");
    }

    public void Callback(PopupYesNoUI.Anwser anwser)
    {
        if (anwser == PopupYesNoUI.Anwser.Yes)
        {
            // Deletes the network from the database
            DB_Controller.DeletDeepNet(currentlySelectedNetwork.name);

            // Updates UI
            Destroy(currentlySelectedNetwork);
        }
    }

    public void GoBackButton()
    {
        // Loads home scene
        FindObjectOfType<SceneLoader>().LoadScene("Home");
    }
}
