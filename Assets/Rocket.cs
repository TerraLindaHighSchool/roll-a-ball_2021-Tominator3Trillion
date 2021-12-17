using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 100f;
    public float maxDistance = 100f;
    public float explosionScale = 1f;
    public AudioClip explosionSound;
    public GameObject explosionPrefab;
    private AudioSource audioSource;

    public Vector3 target;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the rocket to look at the target
        transform.LookAt(target);
        //move towards the target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, target) < 0.1f)
        {
            Explode();
        }
    }

    //on collision Enter
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Tag: " + collision.gameObject.tag);

        if(collision.gameObject.tag != "Camera") {
            Explode();
            

            
            
        }
    }

    private void Explode() {
        audioSource.PlayOneShot(explosionSound);
            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            //add an audio source to the explosion
            explosion.AddComponent<AudioSource>();
            explosion.GetComponent<AudioSource>().clip = explosionSound;
            explosion.GetComponent<AudioSource>().spatialBlend = 0.75f;
            //set the volume rolloff the linear
            explosion.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
            explosion.GetComponent<AudioSource>().Play();

            //scale the explosion
            explosion.transform.localScale = new Vector3(explosionScale,explosionScale,explosionScale);
            //slow down the particle play speed of the explosion

            //call couroutine explosionSlow()
            ParticleSystem[] particles = explosion.GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem particle in particles)
            {
                var main = particle.main;
                main.simulationSpeed = 1f/explosionScale;
            }


            //get the main camera
            Camera mainCamera = Camera.main;
            //get the parent of the parent and the shake script on it
            CameraShake cameraShake = mainCamera.transform.parent.GetComponent<CameraShake>();
            //call the shake method on the camera shake script as a courotine
            //StartCoroutine(cameraShake.Shake(0.5f*explosionScale, 2f*explosionScale));

            Destroy(explosion, 2f * explosionScale);
            Destroy(gameObject);
    }


}
