using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 100f;
    public AudioClip explosionSound;
    public GameObject explosionPrefab;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //on collision Enter
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Tag: " + collision.gameObject.tag);

        if(collision.gameObject.tag != "Camera") {
            audioSource.PlayOneShot(explosionSound);
            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
            Destroy(explosion, 2f);
        }
    }
}
