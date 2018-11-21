using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Fight : NetworkBehaviour {
    public string sceneName = "FourStar";
    public void fight()
    {
        GameObject.FindGameObjectWithTag("Net").GetComponent<NetworkManager>().ServerChangeScene(sceneName);
    }
}
