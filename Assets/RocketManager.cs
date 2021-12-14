using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketManager : MonoBehaviour
{
    //public Image cursorImage;
    private long startClickTime;
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
            
            startClickTime = System.DateTime.Now.Ticks;
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

        if (startClickTime!= 0 && System.DateTime.Now.Ticks - startClickTime > 10000) {
            startClickTime = 0;
            audioSource.PlayOneShot(launchSound);
            GameObject rocket = Instantiate(rocketPrefab, transform.position, new Quaternion(180,0,0,0));
            //rotate teh rocket towards the mouse position
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 direction = mousePosWorld - transform.position;
            direction.y = 0;
            direction.Normalize();
            rocket.transform.rotation = Quaternion.LookRotation(direction);
            //add force to the rocket
            rocket.GetComponent<Rigidbody>().AddForce(direction * 10f);
            GameObject rocketTrail = Instantiate(rocketTrailPrefab, transform.position, transform.rotation);
            Destroy(rocketTrail, 2f);
            //rocket.GetComponent<Rocket>().SetTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        }
    }
}
