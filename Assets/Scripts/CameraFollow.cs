using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject target;
    public float cameraDistance = 10f;
    private float cDist;
    public float cameraHeight = 5f;
    public float smoothTime = 3f;
    private Vector3 velocity = Vector3.zero;

    private Vector3 offset;

    public Vector3 portalPoint = Vector3.zero;
    private bool reachedPortalPoint = false;

    public float rotateValue = 0f;


    // Start is called before the first frame update
    void Start()
    {
        cDist = cameraDistance;
        offset = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

        
        


        

        if(portalPoint != Vector3.zero)
        {
            if(Vector3.Distance(transform.position, portalPoint) > 20f)
            {
                Debug.Log("Reached portal point");

                //Get camera child
                GameObject camChild = transform.GetChild(1).gameObject;

                //enable the cameraFollow script on the camera
                camChild.GetComponent<CameraFollow>().enabled = true;

                //remove the camera as a child of the portalCam
                camChild.transform.SetParent(null);

                //delete the this 
                Destroy(this.gameObject);



            } else {
                //make a look point 10m below the portal point
                Vector3 lookPoint = new Vector3(portalPoint.x, portalPoint.y - cameraHeight - 1f, portalPoint.z);
                var targetRotation = Quaternion.LookRotation(lookPoint - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / smoothTime);
                if(reachedPortalPoint) {
                    //start moving towards the look point
                    transform.position = Vector3.SmoothDamp(transform.position, lookPoint, ref velocity, smoothTime / 1.5f);

                } else {
                    transform.position = Vector3.SmoothDamp(transform.position, portalPoint, ref velocity, smoothTime / 2.5f);
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
                    cDist = Mathf.Lerp(cDist, 0, Time.deltaTime);
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
