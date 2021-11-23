using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private SphereCollider sc;
    private BoxCollider bc;
    public float speed = 1f;
    public float jumpMultiplier = 1f;
    public float jumpForce = 0f;

    public ReflectionProbe probe;

    public GameObject cam;

    public TextMeshProUGUI scoreText;

    public Transform checkPoint;

    private AudioSource audioSource;

    private string groundType = "none";

    public AudioClip coinCollectSound;
    public AudioClip platformRoll;
    public AudioClip grassRoll;
    public AudioClip stoneRoll;
    public AudioClip woodRoll;
    public AudioClip metalRoll;

    public AudioClip land;

    private float downwardVelocity = 0f;

    private float lastGroundedTime = 0;

    private bool touchingPortal = false;





    // Start is called before the first frame update
    void Start()
    {
        rb  = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        bc = GetComponent<BoxCollider>();
        bc.enabled = false;
        audioSource = GetComponent<AudioSource>();

        transform.position = checkPoint.position;
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

       //update probe reflection map

       if(!touchingPortal) {
           probe.RenderProbe();
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
        else if (Input.GetKey(KeyCode.F) && IsGrounded())
        {
            //move the velocity towards 0 with lerp
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime*speed);
        } else if (Input.GetKey(KeyCode.R))
        {
            //reset the position of the player to the checkpoint
            transform.position = checkPoint.position;
            rb.velocity = Vector3.zero;

            transform.rotation = Quaternion.Euler(0, 0, 0);
            rb.angularVelocity = Vector3.zero;
        }


        //based on the velocity of the rigidbody, increase or decrease the feild of view
        if(cam.gameObject.GetComponent<CameraFollow>().portalPoint != Vector3.zero) {
            //get camera child of cam object
            Camera camChild = cam.transform.GetChild(1).GetComponent<Camera>();
            camChild.fieldOfView = Mathf.Lerp(camChild.fieldOfView, Mathf.Clamp(Mathf.Abs(rb.velocity.magnitude) * 5f, 0f, 50f) + 60, Time.deltaTime);
        }



        //set the audio player volume based on teh velocity of the rigidbody
        audioSource.volume = Mathf.Clamp(Mathf.Abs(rb.velocity.magnitude) * 0.1f, 0f, 1f);

        if(!IsGrounded()) {
            audioSource.volume = 0f;
        }


        switch(groundType) {
            case "none":
                audioSource.clip = null;
                break;
            case "platform":
                audioSource.clip = platformRoll;
                break;    
            case "grass":
                audioSource.clip = grassRoll;
                break;
            case "stone":
                audioSource.clip = stoneRoll;
                break;
            case "wood":
                audioSource.clip = woodRoll;
                break;
            default:
                audioSource.clip = platformRoll;
                break;

        }
        if(audioSource.clip != null && !audioSource.isPlaying && IsGrounded()) {
            audioSource.Play();
        }
        
        //set downward velocity to be the negative of the y velocity
        downwardVelocity = -rb.velocity.y;


        if(transform.position.y < -10f) {
            transform.position = checkPoint.position;
            rb.velocity = Vector3.zero;

            transform.rotation = Quaternion.Euler(0, 0, 0);
            rb.angularVelocity = Vector3.zero;
            
        }
    }

    bool IsGrounded()
    {
        //rwturn wether groundType is not none
        return groundType != "none";
    }

    //on collision enter with a trigger, check if the gameobject has the tag coin
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {

            Debug.Log("Coin");
            
            //increase the score by 1
            if(other.gameObject.GetComponent<CoinController>() != null && !other.gameObject.GetComponent<CoinController>().collected) {
                other.gameObject.GetComponent<CoinController>().Collect();
                scoreText.text = (int.Parse(scoreText.text) + 1).ToString();
                //play the coinCollect sound
                audioSource.PlayOneShot(coinCollectSound);

            }
        

        }

        //check if the gameobject has the default tag
        if (other.gameObject.CompareTag("Platform"))
        {
            groundType = "platform";

            //if velocity is greater than 0.1f, then play the land sound
            if (downwardVelocity > 0.5f && lastGroundedTime + 0.3f < Time.time) {
                //set the volume based on the downward velocity
                audioSource.volume = Mathf.Clamp(Mathf.Abs(downwardVelocity) * 0.1f - 0.5f, 0f, 1f);
                audioSource.PlayOneShot(land);
            }
        }

        if(other.gameObject.CompareTag("Grass")) {
            groundType = "grass";
        }

        if(other.gameObject.CompareTag("Portal")) {
            touchingPortal = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            groundType = "platform";
        }
        if(other.gameObject.CompareTag("Grass")) {
            groundType = "grass";
        }
        if(other.gameObject.CompareTag("Portal")) {
            touchingPortal = true;
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Grass"))
        {
            groundType = "none";
            lastGroundedTime = Time.time;
        }

    }

    
}
