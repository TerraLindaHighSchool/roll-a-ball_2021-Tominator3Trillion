﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private SphereCollider sc;
    private BoxCollider bc;
    public float speed = 1f;
    public float jumpMultiplier = 1f;
    public float jumpForce = 0f;


    // Start is called before the first frame update
    void Start()
    {
        rb  = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        bc = GetComponent<BoxCollider>();
        bc.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(IsGrounded()) {
            //get the value of the horizontal and vertical axis
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            //clamp the value of the horizontal and vertical axis to be between -1 and 1
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            //normalize the movement vector to make it 1
            movement = movement.normalized;
            //apply the movement to the rigidbody smoothly

            rb.AddForce(movement * speed);
        }

        


        //check if space is being held down
        if (Input.GetKey(KeyCode.Space))
        {
            //slowly add force to the jumpforce using lerp to a max of 10
            jumpForce = Mathf.Lerp(jumpForce, 10, Time.deltaTime);
            //slowly shrink the height of this gameobject using lerp to a min of 0.1
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2f, 0.1f, 2f), Time.deltaTime);
            sc.radius = Mathf.Lerp(sc.radius, 0.01f, Time.deltaTime);
            bc.size = Vector3.Lerp(bc.size, new Vector3(1f, 0.1f, 1f), Time.deltaTime*3);
        }

       

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bc.size = new Vector3(0.1f, 0.1f, 0.1f);
            bc.enabled = true;
        }
         //if space is released, then set the jumpForce to 0
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if(IsGrounded())
            {
                
                Debug.Log("jumpForce: " + jumpForce);
                //check if transform.up is closer to Vector3.up than transform.down
                if (Vector3.Dot(transform.up, Vector3.up) > Vector3.Dot(-transform.up, Vector3.up))
                {
                    //add force to the rigidbody
                    rb.AddForce(transform.up * jumpForce * jumpMultiplier, ForceMode.Impulse);
                } else { 
                    rb.AddForce(-transform.up * jumpForce * jumpMultiplier, ForceMode.Impulse);
            }
            }
            
            jumpForce = 0f;
            transform.localScale = new Vector3(1f, 1f, 1f);
            sc.radius = 0.5f;
            bc.size = new Vector3(0.1f, 0.1f, 0.1f);
            bc.enabled = false;
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        // create a ray
        Ray ray = new Ray(transform.position, -Vector3.up);
        Physics.Raycast(ray, out hit, 2f);
        return hit.collider != null;
    }

    
}
