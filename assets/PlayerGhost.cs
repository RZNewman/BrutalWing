using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerGhost : NetworkBehaviour {
    CamMove cam;
    AbilSelector abilities;
    public GameObject playerPre;
    public GameObject CamPre;
    GameMngr gm;
    ServerMngr sv;
    int myConId;

    GameObject spawn;

    PhysicalInput inp;
	// Use this for initialization
	public override void OnStartLocalPlayer () {
        
        if (isLocalPlayer)
        {
            inp = GetComponent<PhysicalInput>();
            gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMngr>();
            CmdAssignConnection();
            //CmdUpdateScore(netId);
            CmdGetSpawn();
            GameObject ab = GameObject.FindGameObjectWithTag("Abil");
            if (ab)
            {
                abilities = ab.GetComponent<AbilSelector>();
                gm.popAs(abilities);
            }
            
        }

        
	}
    public Vector3 getGroundTarget()
    {
        return inp.groundTarget;
    }
    [Command]
    void CmdGetSpawn()
    {
        sv = GameObject.FindGameObjectWithTag("Server").GetComponent<ServerMngr>();
        NetworkInstanceId spawnID = sv.assignSpawn();
        NetworkConnection nc = GetComponent<NetworkIdentity>().connectionToClient;
        
        spawn = NetworkServer.FindLocalObject(spawnID);
        //for (uint i = 0; i < 10; i++)
        //{
        //    print(i + "  cmd  " + NetworkServer.FindLocalObject(new NetworkInstanceId(i)));
        //}
        TargetSetSpawn(nc, spawnID);
        

    }
    [TargetRpc]
    void TargetSetSpawn(NetworkConnection nc, NetworkInstanceId sId)
    {
        //for(uint i =0; i<10; i++)
        //{
        //    print(i + "  rpc  " + ClientScene.FindLocalObject(new NetworkInstanceId(i)));
        //}
        spawn = ClientScene.FindLocalObject(sId);
        //print(spawn);
        //print(sId);
        gm.clientFace = spawn.transform.forward;
        GameObject c = Instantiate(CamPre);
        cam = c.GetComponent<CamMove>();
        inp.setCamera(c.GetComponent<Camera>());
        gm.setCam(c.GetComponent<Camera>());
        cam.setup(spawn);
        

    }

    // Update is called once per frame
    bool spawned = false;
	void Update () {
        if (!isLocalPlayer)
            return;
        inp.Poll();
		if(inp.jump&& !spawned)
        {
            if (spawn)
            {
                spawned = true;
                if (abilities)
                {
                    CmdSpawn(abilities.abilPre1, abilities.abilPre2);
                }
                else
                {
                    CmdSpawn(null, null);
                }
                
            }
            

        }
       
        //CmdSync(inp.export());
        
	}
    [Command]
    void CmdSpawn(string a1, string a2)
    {
        
        GameObject p = Instantiate(playerPre, spawn.transform.position, Quaternion.Euler(0, 0, 0));

        //GameObject p = Instantiate(playerPre, new Vector3(0,3), Quaternion.Euler(0, 0, 0));

        NetworkConnection nc = GetComponent<NetworkIdentity>().connectionToClient;

        NetworkServer.SpawnWithClientAuthority(p,nc);

        //p.GetComponent<NetworkIdentity>().AssignClientAuthority(nc);

        PlayerMover pm = p.GetComponent<PlayerMover>();
        //pm.inp = inp;
        pm.ghost = this;
        if (a1!=null)
        {
            GameObject a1Pre = Resources.Load("attacks/" + a1) as GameObject;
            GameObject a2Pre = Resources.Load("attacks/" + a2) as GameObject;
            pm.abilPre1 = a1Pre;
            pm.abilPre2 = a2Pre;
        }
        

        TargetSpawn(nc,p.GetComponent<NetworkIdentity>().netId );



    }

    [TargetRpc]
    void TargetSpawn(NetworkConnection nc, NetworkInstanceId pId)
    {
        //if (!isLocalPlayer)
        //    return;
        GameObject p = ClientScene.FindLocalObject(pId);
        PlayerMover pm = p.GetComponent<PlayerMover>();
        pm.inp = inp;
        pm.ghost = this;
        spawned = true;
        cam.target = p;

    }
    [Command]
    void CmdSync(InputHandler.data d)
    {
        inp.sync(d);
    }
    public Vector3 spawnMutate(Vector3 move)
    {
        return move.x * spawn.transform.right + move.y * Vector3.up + move.z * spawn.transform.forward;
    }
    public void renderCD(float a1CD, float a2CD)
    {
        if (abilities)
        {
            gm.renderCD(a1CD, a2CD);
        }
        
    }
    public void charDied(NetworkInstanceId id)
    {
        spawned = false;
        if (isLocalPlayer)
        {
            CmdUpdateScore(id);
        }
    }
    [Command]
    void CmdUpdateScore(NetworkInstanceId id)
    {
        if(id.IsEmpty())
        {
            //lastHit = -1;
            //do nothing
        }
        else if (id == netId)
        {

            sv.addScore(myConId);

        }
        else
        {
            NetworkServer.FindLocalObject(id).GetComponent<PlayerGhost>().sendScore();
            
        }
        
        
    }
    [Server]
    void sendScore()
    {
        sv.addScore(myConId);
    }

    [Command]
    void CmdAssignConnection()
    {
        sv = GameObject.FindGameObjectWithTag("Server").GetComponent<ServerMngr>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMngr>();
        myConId = GetComponent<NetworkIdentity>().connectionToClient.connectionId;
        gm.assignConnection(GetComponent<NetworkIdentity>().connectionToClient, netId);
        sv.register(myConId);
    }

    void OnDestroy()
    {
        if (isLocalPlayer)
        {
            if (cam)
            {
                Destroy(cam.gameObject);
            }
            
        }
        
    }

}
