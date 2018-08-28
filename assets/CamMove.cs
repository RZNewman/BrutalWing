using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour {
    public GameObject target;
    public Vector3 offset;
    public Vector3 rotation;
	// Use this for initialization
	void Start () {
        transform.position = offset;
        transform.rotation = Quaternion.Euler(rotation);
	}
	
	// Update is called once per frame
	void Update () {
        if (target)
        {
            transform.position = target.transform.position + offset;
        }
        
	}



}
