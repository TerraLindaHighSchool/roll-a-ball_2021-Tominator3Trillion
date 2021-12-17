using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketManager : MonoBehaviour
{
    //public Image cursorImage;
    private float startClickTime;
    public GameObject bombPrefab;
    public GameObject rocketPrefab;
    public GameObject rocketTrailPrefab;
    public AudioClip launchSound;
    public AudioClip lockOnSound;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //set the cursor image to cursorImage
        //Cursor.SetCursor(cursorImage.sprite.texture, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButtonDown(1))
        {
            
            startClickTime = 0;
            //Shoot a ray from the camera to the mouse position and get the position of the hit
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            GameObject bomb;
            if(Physics.Raycast(ray, out hit))
            {
                bomb = Instantiate(bombPrefab, hit.point + new Vector3(0f,100f,0f), new Quaternion(180,0,0,0));
            }
        }
        if(Input.GetMouseButtonDown(0))
        {
            //miliseconds since the last click
            startClickTime = Time.time;
            audioSource.clip = lockOnSound;
            audioSource.Play();
        }
        if(Input.GetMouseButtonUp(0))
        {
            startClickTime = 0;
            if(audioSource.clip == lockOnSound)
            {
                audioSource.Stop();
            }
        }
        if (startClickTime!= 0 && (Time.time - startClickTime) > 1f) {
            startClickTime = 0;
            //disable parent collider
            transform.parent.GetComponent<Collider>().enabled = false;
            GetComponent<Collider>().enabled = false;
            GameObject rocket = null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if(Physics.Raycast(ray, out hit))
            {
                rocket = Instantiate(rocketPrefab, transform.position, Quaternion.identity);
                rocket.GetComponent<Rocket>().target = hit.point;
            }
            transform.parent.GetComponent<Collider>().enabled = true;
            GetComponent<Collider>().enabled = true;
            //add force to the rocket
            GameObject rocketTrail = Instantiate(rocketTrailPrefab, transform.position, transform.rotation);
            Destroy(rocketTrail, 1f);
            //rocket.GetComponent<Rocket>().SetTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        }
    }
}
