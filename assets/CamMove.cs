using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour {
    public GameObject target;
    public Vector3 offset;

    Vector3 offsetReal;

    GameObject click;
	// Use this for initialization
	void Awake () {
        click = transform.GetChild(0).gameObject;
        click.transform.position = transform.position-offset;

    }
    public void setup(GameObject spawn)
    {
        offsetReal = offset.x* spawn.transform.right+ offset.y*Vector3.up+ offset.z*spawn.transform.forward;
        transform.position = spawn.transform.position +offsetReal;
        //print(spawn);
        //transform.rotation = Quaternion.Euler(rotation);
        transform.rotation = Quaternion.LookRotation(spawn.transform.forward+spawn.transform.up*-5);
        click.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            //print(offsetReal);
            transform.position = target.transform.position + offsetReal;
        }

    }


}
