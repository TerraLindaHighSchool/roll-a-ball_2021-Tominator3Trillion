using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public MonoBehaviour script;
    public string scriptFunctionName;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    //while player in collider
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //if player presses E
            if (Input.GetKeyDown(KeyCode.F))
            {
                //call pressButton in the animator
                animator.SetTrigger("pressButton");
                //call function in script
                script.SendMessage(scriptFunctionName);

                // //play animation
                // animation.Play();
                // //call the function in the script
                // script.Invoke(scriptFunctionName, 0f);

            }
        }
    }

}
