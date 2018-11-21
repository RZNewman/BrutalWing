using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProjectileSplit : Projectile {
    public GameObject SplitPre;
    public int numSplit;
    public float angle;
    protected override void endLife()
    {
        float startAngle = (numSplit - 1) * angle/-2;
        for(int i =0; i < numSplit; i++)
        {
            float instanceAngle = startAngle + (i * angle);

            GameObject p = Instantiate(SplitPre);

            p.transform.position += transform.position;

            
            p.transform.rotation = transform.rotation;
            p.transform.Rotate(new Vector3(0, instanceAngle, 0));
            Projectile proj = p.GetComponent<Projectile>();
            proj.setPlayer(ownerTeam, owner);
            NetworkServer.Spawn(p);
        }
      
        base.endLife();
    }
}
