using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraFollow : MonoBehaviour
{


    public GameObject target;
    public float cameraDistance = 10f;
    private float cDist;
    public float cameraHeight = 5f;
    public float smoothTime = 3f;
    public float portalSmooth = 3f;

    public AudioClip portalPassSound;
    public AudioClip portalRampSound;
    private AudioSource audioSource;
    private Vector3 velocity = Vector3.zero;

    private Vector3 offset;


    public Vector3 portalPoint = Vector3.zero;
    private bool reachedPortalPoint = false;

    public float rotateValue = 0f;

    private bool inPortalMovement = false;


    public string[] levelNames;
    public List<Vector3> portalPositions;
    public TextMeshProUGUI levelNameText;
    private int currentLevel = 0;

    private bool inMapMode = false;
    public GameObject mapPostprocessing;

    


    // Start is called before the first frame update
    void Start()
    {
        cDist = cameraDistance;
        offset = new Vector3(0, 0, 0);
        audioSource = GetComponent<AudioSource>();
        mapPostprocessing.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {   
        //if keyinput is M, turn on inMapMode
        if (Input.GetKeyDown(KeyCode.M))
        {
            inMapMode = !inMapMode;
            mapPostprocessing.SetActive(inMapMode);
            target.GetComponent<PlayerController>().enabled = !inMapMode;
            gameObject.GetComponent<PortalTraveller>().enabled = !inMapMode;
        }

        if(inMapMode) {
            
            //move the camera 40 units above the target and turn the camera to look at the target
            
            //lerp the position
            transform.position = Vector3.Lerp(transform.position, target.transform.position + new Vector3(0, 150, 0), Time.deltaTime * portalSmooth);
            transform.LookAt(target.transform.position);

            //lerp the field of view to 150
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 40, Time.deltaTime * portalSmooth);

            return;
            
        }

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 90, Time.deltaTime * portalSmooth);

        
        
        Camera.main.GetComponent<Camera>().fieldOfView = Mathf.Lerp(Camera.main.GetComponent<Camera>().fieldOfView, 60f, Time.deltaTime * 1.5f);

        

        if(portalPoint != Vector3.zero)
        {
            if(!inPortalMovement)
            {
                //play portal ramp sound
                audioSource.clip = portalRampSound;
                audioSource.Play();
                inPortalMovement = true;
                
                //set the level to the closest portal position in the list
                currentLevel = 0;
                float closestDistance = Vector3.Distance(portalPositions[0], portalPoint);
                for(int i = 1; i < portalPositions.Count; i++)
                {
                    float distance = Vector3.Distance(portalPositions[i], portalPoint);
                    if(distance < closestDistance)
                    {
                        closestDistance = distance;
                        currentLevel = i;
                    }
                }
            



                levelNameText.text = levelNames[currentLevel];
                levelNameText.enabled = true;
                //set opacity to 0
                levelNameText.color = new Color(levelNameText.color.r, levelNameText.color.g, levelNameText.color.b, 0f);
                //set the current level to the index of portalPoint in the vector3 array called portalPositions
                
                
            }


            if(Vector3.Distance(transform.position, portalPoint) > 20f)
            {
                reachedPortalPoint = false;
                portalPoint = Vector3.zero;

                //play portal pass sound
                audioSource.clip = portalPassSound;
                audioSource.Play();
 
                levelNameText.enabled = false;
                inPortalMovement = false;
                portalSmooth = 1.5f; // make transitions quicker after the first portal

                target.GetComponent<PlayerPortalPhysics>().graphicsObject = target.GetComponent<PlayerPortalPhysics>().graphicsClone;



            } else {

                //lerp the level name text to full opacity
                levelNameText.color = new Color(levelNameText.color.r, levelNameText.color.g, levelNameText.color.b, Mathf.Lerp(levelNameText.color.a, 0.95f, Time.deltaTime * 2f));

                Debug.Log("Not yet reached portal point");
                //make a look point 10m below the portal point
                Vector3 lookPoint = new Vector3(portalPoint.x, portalPoint.y - cameraHeight - 1f, portalPoint.z);
                var targetRotation = Quaternion.LookRotation(lookPoint - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / smoothTime);
                if(reachedPortalPoint) {
                    //start moving towards the look point
                    transform.position = Vector3.SmoothDamp(transform.position, lookPoint, ref velocity, portalSmooth / 1.5f);

                } else {
                    transform.position = Vector3.SmoothDamp(transform.position, portalPoint, ref velocity, portalSmooth / 2.5f);
                    //blur the camera the closer it gets to the portal point
                    float dist = Vector3.Distance(transform.position, portalPoint);

                    Camera.main.GetComponent<Camera>().fieldOfView = Mathf.Lerp(Camera.main.GetComponent<Camera>().fieldOfView, 60f - (dist * 2f), Time.deltaTime * 1.5f);
                    // if(dist < 3f)
                    // {
                    //     Camera.main.GetComponent<Camera>().fieldOfView = Mathf.Lerp(Camera.main.GetComponent<Camera>().fieldOfView, 10f, Time.deltaTime /portalSmooth);
                    // } else {
                    //     Camera.main.GetComponent<Camera>().fieldOfView = Mathf.Lerp(Camera.main.GetComponent<Camera>().fieldOfView, 100f, Time.deltaTime * 1.5f);
                    // }
                    if(Vector3.Distance(transform.position, portalPoint) < 0.1f)
                    {
                        reachedPortalPoint = true;
                    }
                }
            }
        } else {
             //rotate the camera around the target on its z axis with the q and e keys
            if (Input.GetKey(KeyCode.Q))
            {
                rotateValue += Time.deltaTime * 100f;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                rotateValue -= Time.deltaTime * 100f;
            } 
        
            //set the target rotation
            var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / smoothTime);

            Vector3 targetPosition = target.transform.position + new Vector3(0, cameraHeight, -cDist); // gets the position of the target and adds the camera height and distance to it
            //rotate the targetPosition around the target on its y axis with the rotateValue
            //check copy targetPosition
            Vector3  targetPosition2 = targetPosition;
            targetPosition = RotatePointAroundPivot(targetPosition, target.transform.position, new Vector3(0f,rotateValue,0f));
            if(targetPosition2 != targetPosition)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.5f);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.75f);
            }
            
            
            //if the camera can not draw a direct raycast to the target, then move the camera closer to the target
            RaycastHit hit;
            if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit))
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    //check if target rigidbody is moving
                    if (target.GetComponent<Rigidbody>().velocity.magnitude > 0.5f)
                    {
                        cDist = Mathf.Lerp(cDist, cameraDistance, Time.deltaTime);
                    }
                    
                    
                } else {
                    //make sure the collider is not a trigger
                    if(!hit.collider.isTrigger)
                    {
                        cDist = Mathf.Lerp(cDist, 0, Time.deltaTime);
                    }
                    
                }
            }
            
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime); // moves the camera to the target position


            //if the camera is too far away from the target, move it closer
            if (Vector3.Distance(transform.position, target.transform.position) > cameraDistance*3)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*3);
            }

            //add or substract from the camera distance based on the scroll wheel
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                cameraDistance -= 0.1f;
                //make sure the camera distance is more than 20
                if (cameraDistance < 2)
                {
                    cameraDistance = 2;
                }
                cDist = cameraDistance;
                Debug.Log("Scroll up");
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                cameraDistance += 0.1f;
                //make sure camera distance is not less than 2
                if (cameraDistance > 20)
                {
                    cameraDistance = 20;
                }
                
                cDist   = cameraDistance;
            }
        }
        

        

    }


    // function RotatePointAroundPivot(point: Vector3, pivot: Vector3, angles: Vector3): Vector3 {
    //     var dir: Vector3 = point - pivot; // get point direction relative to pivot
    //     dir = Quaternion.Euler(angles) * dir; // rotate it
    //     point = dir + pivot; // calculate rotated point
    //     return point; // return it
    // }
    //convert to c#
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

}
