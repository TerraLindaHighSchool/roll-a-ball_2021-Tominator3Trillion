using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketManager : MonoBehaviour
{
    //public Image cursorImage;
    private long startClickTime;
    public GameObject rocketPrefab;
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
            GameObject rocket = Instantiate(rocketPrefab, transform.position, transform.rotation);
            //rocket.GetComponent<Rocket>().SetTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        }
    }
}
