using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_ShadowBehaviour : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, .001f, transform.position.z);
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
