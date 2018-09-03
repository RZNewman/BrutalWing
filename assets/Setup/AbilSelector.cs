using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilSelector : MonoBehaviour {
    public GameObject abilPre1, abilPre2;
    public GameObject abilI1, abilI2;
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	public void hold(GameObject abil1, GameObject abil2)
    {
        Ability a1 = abil1.GetComponent<Ability>();
        abilPre1 = a1.AttackPre;
        abilI1 = a1.self;
        Ability a2 = abil2.GetComponent<Ability>();
        abilPre2 = a2.AttackPre;
        abilI2 = a2.self;
    }
}
