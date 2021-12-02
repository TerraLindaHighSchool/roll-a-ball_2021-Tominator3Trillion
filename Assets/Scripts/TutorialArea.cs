using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialArea : MonoBehaviour
{
    public string trueTutorialText;
    public string falseTutorialText;

    public TextMeshProUGUI textMesh;
    public AudioClip trueClip;
    public AudioClip falseClip;
    public AudioSource audioSource;

    public bool isMiniTutorial = false;

    //write player attributes in the inspector
    [Header("Player Attributes")]
    public float playerMinTemperature;





    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && shouldChange(other))
        {
            if(checkConditions(other.gameObject))
            {
                if(textMesh.text != trueTutorialText)
                {
                    textMesh.text = trueTutorialText;
                    audioSource.clip = trueClip;
                    audioSource.Play();
                }

                
            }
            else
            {
                if (textMesh.text != falseTutorialText)
                {
                    textMesh.text = falseTutorialText;
                    audioSource.clip = falseClip;
                    audioSource.Play();
                }
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
