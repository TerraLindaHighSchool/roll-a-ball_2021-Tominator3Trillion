using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{

    private float startingY;

    public bool collected = false;

    // Start is called before the first frame update
    void Start()
    {
        startingY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //bob up and down over time
        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time) * 0.1f + startingY, transform.position.z);
        
        //rotate over time
        transform.Rotate(new Vector3(Time.deltaTime * 50, Time.deltaTime * 50, Time.deltaTime * 50));
    }

    public void Collect() {
        collected = true;
        Destroy(gameObject);
    }
}
