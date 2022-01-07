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

    public GameObject enabledPopup;

    //write player attributes in the inspector
    [Header("Player Attributes")]
    public string requiredCondition;
    public string[] playerConditions;
    



    
    public static Vector3 StringToVector3(string sVector)
     {
         // Remove the parentheses
         if (sVector.StartsWith ("(") && sVector.EndsWith (")")) {
             sVector = sVector.Substring(1, sVector.Length-2);
         }
 
         // split the items
         string[] sArray = sVector.Split(',');
 
         // store as a Vector3
         Vector3 result = new Vector3(
             float.Parse(sArray[0]),
             float.Parse(sArray[1]),
             float.Parse(sArray[2]));
 
         return result;
     }

    private bool checkCondition(string condition, GameObject player) {
        string[] conditionSplit = condition.Split(' ');
        string variableName = conditionSplit[0];
        string valueExpression = conditionSplit[1];
        string variableDesiredValue = conditionSplit[2];
        
        
        object variableValue = player.GetComponent<PlayerController>().StringToVariable(variableName);

        if (variableValue == null) {
            Debug.Log("Variable " + variableName + " not found");
            return false;
        }

        if(variableValue is float) {
            float variableValueFloat = (float)variableValue;
            float variableDesiredValueFloat = float.Parse(variableDesiredValue);

            if(valueExpression.Contains("<")) {
                return variableValueFloat < variableDesiredValueFloat;
            } else if(valueExpression.Contains(">")) {
                return variableValueFloat > variableDesiredValueFloat;
            } else if(valueExpression.Contains("=")) {
                return variableValueFloat == variableDesiredValueFloat;
            }
        } else if(variableValue is int) {
            int variableValueInt = (int)variableValue;
            int variableDesiredValueInt = int.Parse(variableDesiredValue);

            if(valueExpression.Contains("<")) {
                return variableValueInt < variableDesiredValueInt;
            } else if(valueExpression.Contains(">")) {
                return variableValueInt > variableDesiredValueInt;
            } else if(valueExpression.Contains("=")) {
                return variableValueInt == variableDesiredValueInt;
            }
        } else if(variableValue is bool) {
            bool variableValueBool = (bool)variableValue;
            bool variableDesiredValueBool = bool.Parse(variableDesiredValue);

            if(valueExpression.Contains("=")) {
                return variableValueBool == variableDesiredValueBool;
            }
        } else if(variableValue is Vector3 || variableValue is Transform) {
            Vector3 variableValueVector3 = Vector3.zero;
            if(variableValue is Transform) {
                variableValueVector3 = ((Transform)variableValue).position;
            } else {
                variableValueVector3 = (Vector3)variableValue;
            }
            
            if(valueExpression.Contains(">") || valueExpression.Contains("<")) {
                //check if the variableValueVector3 is inside the boxCollider of this
                return GetComponent<BoxCollider>().bounds.Contains(variableValueVector3);
            } else {
                Vector3 variableDesiredValueVector3 = StringToVector3(variableDesiredValue);
            }

        }

        return true;

    }

    public bool checkRequiredCondition(GameObject player) {
        if(requiredCondition == "") {
            return true;
        }
        if(checkCondition(requiredCondition, player)) {
            return true;
        }
        return false;
    }





    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && shouldChange(other))
        {
            if(checkConditions(other.gameObject))
            {
                
                if(textMesh.text != trueTutorialText && checkRequiredCondition(other.gameObject)) {
                    textMesh.text = trueTutorialText;
                    audioSource.clip = trueClip;
                    audioSource.Play();
                    if(enabledPopup != null) {
                        enabledPopup.SetActive(true);
                    }

                }

                
            }
            else
            {
                if (textMesh.text != falseTutorialText && checkRequiredCondition(other.gameObject))
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
            if(enabledPopup != null) {
                enabledPopup.SetActive(false);
            }
            textMesh.text = "";
        }
    }

    private bool checkConditions(GameObject player)
    {
        for(int i = 0; i < playerConditions.Length; i++)
        {
            if(!checkCondition(playerConditions[i], player))
            {
                return false;
            }
        }
        return true;

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
