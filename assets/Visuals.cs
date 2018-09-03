using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Visuals : NetworkBehaviour {
    public SpriteRenderer sprint;
    public MeshRenderer cube;

    public Material normal;
    public Material angry;
	// Use this for initialization
    
    [Command]
	public void CmdSprintAlpha (float per) {
        float a;
        if (per == 1)
        {
            a = 170;
        }
        else
        {
            a = 100 * per;
        }
        //a = 100 * per;
        a = a / 255;
        RpcSprintAlpha(a);
	}
    [ClientRpc]
    void RpcSprintAlpha(float a)
    {
        sprint.color = new Color(1, 1, 1, a);
    }
    [Command]
    public void CmdColorNose(bool atk)
    {
        RpcColorNose(atk);
    }
    [ClientRpc]
	public void RpcColorNose(bool attacking)
    {
        if (attacking)
        {
            cube.material = angry;
        }
        else
        {
            cube.material = normal;
        }
    }
    [Command]
    public void CmdPropagate()
    {
        RpcVis();
    }
    [ClientRpc]
    void RpcVis()
    {

    }
	// Update is called once per frame
	void Update () {
		
	}
}
