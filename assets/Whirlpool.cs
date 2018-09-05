using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Whirlpool : Spawnable {
    public float pullStr;
    public float pullDecay;

    SphereCollider col;
    GameObject ownerObj;
	// Use this for initialization
	protected override void Start2 () {
        col = GetComponent<SphereCollider>();
        if (isClient)
        {
            ownerObj = ClientScene.FindLocalObject(owner);
        }
        else
        {
            ownerObj = NetworkServer.FindLocalObject(owner);
        }
            
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {

            foreach (Collider c in Physics.OverlapSphere(transform.position, col.radius*transform.localScale.x, 1 << 10))
            {
                //print(c);
                if (c.gameObject!= ownerObj && c.GetComponent<PlayerMover>().grounded)
                {
                    //print("owner");
                    Vector3 pos = transform.position;
                    pos.y = c.transform.position.y;//apptly works without

                    Vector3 dif = c.transform.position - transform.position;
                    if (dif.magnitude > 0.2f && dif.y<1f)
                    {
                        //print("pull");
                        float per = (pullStr * Time.fixedDeltaTime) / dif.magnitude;
                        //Debug.Log(per);
                        if (per > 1) per = 1;
                        c.transform.position = Vector3.Lerp(c.transform.position, pos, per);
                    }
                    
                    
                    
                }
            }
            pullStr -= pullDecay * Time.fixedDeltaTime;
        
		

        
	}
}
