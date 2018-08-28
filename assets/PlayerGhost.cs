using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerGhost : NetworkBehaviour {
    CamMove cam;

    public GameObject playerPre;
    public GameObject CamPre;

    PhysicalInput inp;
	// Use this for initialization
	void Start () {
        inp = GetComponent<PhysicalInput>();
        if (isLocalPlayer)
        {
            GameObject c = Instantiate(CamPre,transform);
            cam = c.GetComponent<CamMove>();    
            inp.setCamera(c.GetComponent<Camera>());
        }

        
	}

    // Update is called once per frame
    bool spawned = false;
	void Update () {
        if (!isLocalPlayer)
            return;
        inp.Poll();
		if(inp.jump&& !spawned)
        {
            spawned = true;
            CmdSpawn();

        }
        else
        {
            CmdSync(inp.export());
        }
	}
    [Command]
    void CmdSpawn()
    {
        //Debug.Log("here");
        GameObject p = Instantiate(playerPre, new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 0));

        NetworkConnection nc = GetComponent<NetworkIdentity>().connectionToClient;

        NetworkServer.SpawnWithClientAuthority(p,nc);

        //p.GetComponent<NetworkIdentity>().AssignClientAuthority(nc);

        PlayerMover pm = p.GetComponent<PlayerMover>();
        pm.inp = inp;
        pm.ghost = this;


        TargetSpawn(nc, p);



    }
    [TargetRpc]
    void TargetSpawn(NetworkConnection nc, GameObject p)
    {
        //if (!isLocalPlayer)
        //    return;
        PlayerMover pm = p.GetComponent<PlayerMover>();
        pm.inp = inp;
        pm.ghost = this;

        cam.target = p;

    }
    [Command]
    void CmdSync(InputHandler.data d)
    {
        inp.sync(d);
    }

    public void charDied()
    {
        spawned = false;
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Debug.Log("Disconnected from server: " + info);
        Destroy(cam.gameObject);
    }

}
