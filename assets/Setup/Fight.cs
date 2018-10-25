using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Fight : NetworkBehaviour {

    public void fight()
    {
        GameObject.FindGameObjectWithTag("Net").GetComponent<NetworkManager>().ServerChangeScene("FourStar");
    }
}
