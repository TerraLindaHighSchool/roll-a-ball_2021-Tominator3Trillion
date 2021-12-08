using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockToObject : MonoBehaviour
{

    public bool keepCurrentOffset = false;
    private Vector3 offset = Vector3.zero;

    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        if(keepCurrentOffset)
        {
            offset = transform.position - target.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.transform.position + offset;
    }
}
