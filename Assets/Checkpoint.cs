using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public GameObject player;
    private float startingY;

    // Start is called before the first frame update
    void Start()
    {
        startingY = player.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //set the renderer to enabled or disabled if the player is within 15m of the checkpoint
        if (Vector3.Distance(player.transform.position, transform.position) < 20f)
        {
            GetComponent<Renderer>().enabled = true;
            //Slowly move the z position towards the startingZ position based on the distance between the player and the checkpoint
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(startingY+10f, startingY-2f, 1f-(Vector3.Distance(player.transform.position, transform.position)/25f)),transform.position.z);
            
            
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
            transform.position = new Vector3(transform.position.x, startingY + 10f, transform.position.z);
        }
    }
}
