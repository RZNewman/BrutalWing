using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Blink : Spawnable
{
    public float distance = 2.5f;
    GameObject ownerObj;
    // Use this for initialization
    protected override void Start2()
    {
        if (isClient)
        {
            ownerObj = ClientScene.FindLocalObject(owner);
            if (ownerObj.GetComponent<NetworkIdentity>().hasAuthority)
            {
                ownerObj.transform.position += ownerObj.transform.forward * distance;
            }
        }

    }
}
