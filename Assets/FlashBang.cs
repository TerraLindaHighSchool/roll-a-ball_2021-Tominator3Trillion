using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FlashBang : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip bangSound;
    public float duration = 1f;
    public float falloff = 0.5f;
    private bool isFlashing = false;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Flash()
    {
        StartCoroutine(FlashBangCoroutine());
    }

    //create a coroutine to flash the screen
    IEnumerator FlashBangCoroutine()
    {
        AutoExposure autoExposureLayer;
        PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
        //set the exposure to 1000 for a 3 second duration and then lerp it to 0 over the duration multiplied by the falloff
        volume.profile.TryGetSettings(out autoExposureLayer);
        autoExposureLayer.active = true;
        
        autoExposureLayer.keyValue.value = 1000f;
        
        //wait for the duration
        yield return new WaitForSeconds(duration);
        
        // //lerp the exposure to 0 over the duration multiplied by the falloff
        while(autoExposureLayer.keyValue.value > 0.5f) {
            autoExposureLayer.keyValue.value = Mathf.Lerp(autoExposureLayer.keyValue.value, 0f, Time.deltaTime * falloff);
            yield return new WaitForSeconds(0.001f);
        }
         


        autoExposureLayer.active = false;
    }
}
