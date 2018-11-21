using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Buff : NetworkBehaviour {

    public Dictionary<string, float> mods;
    public float duration;
    [HideInInspector]
    [SyncVar]
    public NetworkInstanceId owner;
    float birth;
    BuffableLoose holder;

    void Start()
    {
        mods = new Dictionary<string, float>();
        holder = GetComponentInParent<BuffableLoose>();
        holder.addBuff(this);
        birth = Time.time;
        AddMods();
    }
    protected abstract void AddMods();
    public override void OnStartClient()
    {
        GameObject par = ClientScene.FindLocalObject(owner);
        transform.parent = par.transform;
        transform.position = transform.parent.position;
        StartupClient();
    }
    protected abstract void StartupClient();

    private void Update()
    {
        if (Time.time > birth + duration)
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        holder.removeBuff(this);
    }

    // Use this for initialization

}
