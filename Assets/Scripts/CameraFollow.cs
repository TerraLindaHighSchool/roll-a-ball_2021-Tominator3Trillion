using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject target;
    public float cameraDistance = 5f;
    private float cDist;
    public float cameraHeight = 5f;
    public float smoothTime = 3f;
    private Vector3 velocity = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        cDist = cameraDistance;
    }

    // Update is called once per frame
    void Update()
    {
        //smoothly look at the target
        var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
       
        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime * Time.deltaTime);


        

        
        Vector3 targetPosition = target.transform.position + new Vector3(0, cameraHeight, -cDist); // gets the position of the target and adds the camera height and distance to it
        
        
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




        

    }
}
