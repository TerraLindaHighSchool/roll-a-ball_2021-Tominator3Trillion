using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialArea : MonoBehaviour
{
    public string trueTutorialText;
    public string falseTutorialText;

    public TextMeshProUGUI textMesh;

    public bool isMiniTutorial = false;

    //write player attributes in the inspector
    [Header("Player Attributes")]
    public float playerMinTemperature;



    //on collider enter with player
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && shouldChange(other))
        {
            if(checkConditions(other.gameObject))
            {
                textMesh.text = trueTutorialText;
            }
            else
            {
                textMesh.text = falseTutorialText;
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && shouldChange(other))
        {
            if(checkConditions(other.gameObject))
            {
                textMesh.text = trueTutorialText;
            }
            else
            {
                textMesh.text = falseTutorialText;
            }
        }
    }

    //on collider exit with player
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && shouldChange(other))
        {
            textMesh.text = "";
        }
    }

    private bool checkConditions(GameObject player)
    {
        if(player.GetComponent<PlayerController>().playerTemp > playerMinTemperature)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private bool shouldChange(Collider other) {
        if (isMiniTutorial) {
            return true;
        }
        else {
            //check if the text is blank
            if (textMesh.text == "" || textMesh.text == falseTutorialText || textMesh.text == trueTutorialText) {
                return true;
            }
            else {
                return false;
            }
        }
    }

}
