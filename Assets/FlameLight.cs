using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameLight : MonoBehaviour
{
    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        //randomly fluctuate the brightness of the light based on time
        light.intensity = Random.Range(12f, 15f) + Mathf.Sin(Time.time * 2.0f);

    }
}
