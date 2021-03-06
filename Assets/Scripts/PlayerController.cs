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
    public GameObject checkPointParticle;
    public GameObject respawnParticle;
    private float timeSinceRespawn;

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
    public float currentVelocity = 0f;

    private float lastGroundedTime = 0;

    private bool touchingPortal = false;

    [SerializeField]public float playerTemp = 0f;

    public Renderer renderer;

    public Color glowingColor;
    public Light glowLight;

    private Color originalColor;
    private bool inFire = false;

    //bomb and rocket count TMP
    public TextMeshProUGUI bombCountText;
    public TextMeshProUGUI rocketCountText;


    [SerializeField]public int bombCount = 0;
    [SerializeField]public int rocketCount = 0;

    public GameObject rocketManager;
    public bool inMapMode = false;

    



    



    public object StringToVariable(string name) {
        return this.GetType().GetField(name).GetValue(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        bombCountText.text = "Bombs: " + bombCount;
        rocketCountText.text = "Rockets: " + rocketCount;
        timeSinceRespawn = Time.time;
        rb  = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        bc = GetComponent<BoxCollider>();
        bc.enabled = false;
        audioSource = GetComponent<AudioSource>();
        originalColor = renderer.material.color;

        transform.position = checkPoint.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //set map to if rocketManger is active
        inMapMode = rocketManager.activeInHierarchy;
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
            //make a timeMultiplier based on the player temp
            float timeMultiplier = playerTemp / 150f;
            //slowly add force to the jumpforce using lerp to a max of 10
            jumpForce = Mathf.Lerp(jumpForce, 10, Time.deltaTime*timeMultiplier);
            //slowly shrink the height of this gameobject using lerp to a min of 0.1
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2f, 0.1f, 2f), Time.deltaTime*timeMultiplier);
            sc.radius = Mathf.Lerp(sc.radius, 0.01f, Time.deltaTime*timeMultiplier);
            bc.size = Vector3.Lerp(bc.size, new Vector3(1f, 0.1f, 1f), Time.deltaTime*3*timeMultiplier);
        }

       //update probe reflection map

       if(!touchingPortal) {
           probe.RenderProbe();
       }


    }

    void Update()
    {
        bombCountText.text = "Bombs: " + bombCount;
        rocketCountText.text = "Rockets: " + rocketCount;
        currentVelocity = rb.velocity.magnitude;
        if (!inFire) {
            playerTemp = Mathf.Lerp(playerTemp, 0f, Time.deltaTime/10f);
            if(playerTemp < 30f) {
                playerTemp = Mathf.Lerp(playerTemp, 0f, Time.deltaTime);
            }
        }
            

         if(playerTemp < 1f) {
             playerTemp = 0f;
             glowLight.intensity = 0f;
         } else {
             glowLight.intensity = playerTemp/255f*3f;
         }
        

        

        //set the renderer color based on the player temp
        renderer.material.color = Color.Lerp(originalColor, glowingColor, playerTemp/255f);
        //lerp the emmision color of the renderer based on the player temp
        renderer.material.SetColor("_EmissionColor", Color.Lerp(Color.black, glowingColor, playerTemp/255f));

        //set the light intensity based on the player temp
        


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
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime*speed*2);
        } else if (Input.GetKey(KeyCode.R))
        {
            //reset the position of the player to the checkpoint
            if(Time.time-timeSinceRespawn > 1f) {
                Respawn();
            }
            
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
            Respawn();
            
        }
        

    }


    void Respawn() {
        transform.position = checkPoint.position;
        rb.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rb.angularVelocity = Vector3.zero;
        GameObject g = Instantiate(respawnParticle, transform.position, Quaternion.identity);
        //set size to 0.9
        g.transform.parent = transform;
        timeSinceRespawn = Time.time;
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

        if (other.gameObject.CompareTag("BombToken") && other.gameObject.GetComponent<CoinController>() != null && !other.gameObject.GetComponent<CoinController>().collected) {
            bombCount++;
            bombCountText.text = "Bombs: " + bombCount;
            other.gameObject.GetComponent<CoinController>().Collect();
            Destroy(other.gameObject);
            //audioSource.PlayOneShot(bombCollectSound);

        } else if (other.gameObject.CompareTag("RocketToken") && other.gameObject.GetComponent<CoinController>() != null && !other.gameObject.GetComponent<CoinController>().collected)
        {
            other.gameObject.GetComponent<CoinController>().Collect();
            rocketCount++;
            rocketCountText.text = "Rockets: " + rocketCount;
            Destroy(other.gameObject);
            //audioSource.PlayOneShot(rocketCollectSound);
        }
        

        

        //check if the gameobject has the default tag
        if (other.gameObject.CompareTag("Platform"))
        {
            groundType = "platform";

            //if velocity is greater than 0.1f, then play the land sound
            if (downwardVelocity > 0.2f && lastGroundedTime + 0.3f < Time.time) {
                //set the volume based on the downward velocity
                audioSource.volume = Mathf.Clamp(Mathf.Abs(downwardVelocity) * 0.1f - 0.2f, 0f, 1f);
                audioSource.PlayOneShot(land);
            }
        }

        else if(other.gameObject.CompareTag("Grass")) {
            groundType = "grass";
        }

        else if(other.gameObject.CompareTag("Portal")) {
            touchingPortal = true;
        } else if (other.gameObject.CompareTag("Fire"))
        {
            inFire = true;
        }

        else if (other.gameObject.CompareTag("CheckPoint"))
        {
            checkPoint = other.gameObject.transform;
            //disable the other
            other.gameObject.SetActive(false);
            //instantiate the checkPointParticle
            GameObject cp = Instantiate(checkPointParticle, other.gameObject.transform.position, Quaternion.identity);
            //destroy the particle after 2 seconds
            Destroy(cp, cp.GetComponent<ParticleSystem>().main.duration-0.1f);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            groundType = "platform";
        }
        else if(other.gameObject.CompareTag("Grass")) {
            groundType = "grass";
        }
        else if(other.gameObject.CompareTag("Portal")) {
            touchingPortal = true;
        }

        //check if tag is "Fire"
        else if (other.gameObject.CompareTag("Fire"))
        {
            //if the player is touching the fire lerp the player temperature towards 255
            playerTemp = Mathf.Lerp(playerTemp, 255f, Time.deltaTime/4f);

        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Grass"))
        {
            groundType = "none";
            lastGroundedTime = Time.time;
        } else if (other.gameObject.CompareTag("Fire"))
        {
            inFire = false;
        }

    }

    
}
