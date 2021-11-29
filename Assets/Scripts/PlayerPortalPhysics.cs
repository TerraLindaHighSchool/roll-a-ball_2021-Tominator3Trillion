using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent (typeof (Rigidbody))]
public class PlayerPortalPhysics : PortalTraveller {

    public float force = 10;
    new Rigidbody rigidbody;
    public Color[] colors;
    static int i;

    public PlayerController player;

    public GameObject portalCamPrefab;





    void Awake () {
        rigidbody = GetComponent<Rigidbody> ();
        graphicsObject.GetComponent<MeshRenderer> ().material.color = colors[i];
        i++;
        if (i > colors.Length - 1) {
            i = 0;
        }
    }

    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        base.Teleport (fromPortal, toPortal, pos, rot);
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;



        // //Instantiat a portal cam
        // GameObject portalCam = Instantiate (portalCamPrefab, player.cam.transform.position, player.cam.transform.rotation);

        // //remove the cameraFollow script from the camera
        // player.cam.GetComponent<CameraFollow> ().rotateValue = 0f;
        // player.cam.GetComponent<CameraFollow> ().enabled = false;
        //set the portalCam target to this
        //player.cam.GetComponent<CameraFollow> ().target = transform.gameObject;

        //add the cam as a child of the portalCam
        //player.cam.transform.SetParent (portalCam.transform);


        player.cam.GetComponent<CameraFollow> ().portalPoint = new Vector3 (fromPortal.position.x + 2f, fromPortal.position.y + player.cam.GetComponent<CameraFollow> ().cameraHeight, fromPortal.position.z);
        
        GetComponent<PlayerController>().checkPoint = toPortal;
        
    }
}
