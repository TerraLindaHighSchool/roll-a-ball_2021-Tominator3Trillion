using System.Collections;
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

    public Camera cam;


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

            //change the movment vector to be the same as the camera
            movement = cam.transform.TransformDirection(movement);

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

            //set the transform so that it is vertaical to the z axis
            transform.rotation = Quaternion.Euler(0, 0, 0);
            //remove rigidbody rotation velocity
            rb.angularVelocity = Vector3.zero;
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

        //based on the velocity of the rigidbody, increase or decrease the feild of view
        if(cam.gameObject.GetComponent<CameraFollow>().portalPoint != Vector3.zero) {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Clamp(Mathf.Abs(rb.velocity.magnitude) * 5f, 0f, 50f) + 60, Time.deltaTime);
        }


        
    }

    bool IsGrounded()
    {
        //return wether the player is colliding with anything
        return Physics.CheckCapsule(sc.bounds.center, new Vector3(sc.bounds.center.x, sc.bounds.min.y, sc.bounds.center.z), sc.radius * 1.1f, LayerMask.GetMask("Default"));
    }

    
}
