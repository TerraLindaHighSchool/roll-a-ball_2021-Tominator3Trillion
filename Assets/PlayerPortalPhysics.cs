using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PlayerPortalPhysics : PortalTraveller {

    public float force = 10;
    new Rigidbody rigidbody;
    public Color[] colors;
    static int i;

    public PlayerController player;

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
        rigidbody.velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (rigidbody.velocity));
        rigidbody.angularVelocity = toPortal.TransformVector (fromPortal.InverseTransformVector (rigidbody.angularVelocity));



        GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
        //set the position of the sphere to the position of the cam
        sphere.transform.position = player.cam.transform.position;
        //set the sphere as a parent of the cam
        player.cam.transform.parent = sphere.transform;

        //add the script PortalTraveller to the sphere
        sphere.AddComponent<PortalTraveller> ();

        //set the graphics object of the sphere to itself
        sphere.GetComponent<PortalTraveller> ().graphicsObject = player.cam.gameObject;



        player.cam.gameObject.GetComponent<CameraFollow> ().portalPoint = new Vector3 (fromPortal.position.x, fromPortal.position.y + player.cam.gameObject.GetComponent<CameraFollow> ().cameraHeight, fromPortal.position.z);


        
    }
}
