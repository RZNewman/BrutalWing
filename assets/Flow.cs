using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flow : Buff {

    protected override void AddMods()
    {
        mods.Add("FullSprint", 1);
    }

    protected override void StartupClient () {
        
        
        PlayerMover pm = GetComponentInParent<PlayerMover>();
        pm.setSprint(true);
        
        
	}

}
