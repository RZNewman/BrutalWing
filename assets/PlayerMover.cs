using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMover : NetworkBehaviour {
    public InputHandler inp;
    public GameObject attackPre1;
    public GameObject attackPre2;
    public GameObject abilPre1;
    public GameObject abilPre2;
    public PlayerGhost ghost;

    Rigidbody rb;
    CapsuleCollider col;
    Health hp;
    PState current;
    LookState look;
    GameObject attack;
    Visuals vis;
    BuffableLoose buffholder;


    enum PState
    {
        Free,
        Air,
        KB
    }
    enum LookState
    {
        Free,
        Attacking
    }
    enum attackType
    {
        basic,
        heavy,
        abil1,
        abil2
    }

    GameObject lookup(attackType a)
    {
        switch (a)
        {
            case attackType.basic:
                return attackPre1;
            case attackType.heavy:
                return attackPre2;
            case attackType.abil1:
                return abilPre1;
            case attackType.abil2:
                return abilPre2;
            default:
                return attackPre1;
        }
            
    }
    void setCD(attackType a, float cd)
    {
        switch (a)
        {
            case attackType.abil1:
                abil1CD = cd;
                break;
            case attackType.abil2:
                abil2CD = cd;
                break;
        }
    }
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        hp = GetComponent<Health>();
        current = PState.Free;
        look = LookState.Free;
        
        vis = GetComponent<Visuals>();
        buffholder = GetComponent<BuffableLoose>();

    }

    // Update is called once per frame
    public float moveForce = 600;
    public float maxSpeed = 3;
    public float jumpForce = 20;
    public float KBDeteriorate = 2;
    public float KBRegain = 2;
    public float sprintModHalf = 0.3f;
    public float sprintModFull = 0.7f;
    public float sprintBuild = 2;
    public float sprintChangeLoss = 0.3f;

    float sprintBuildCurrent = 0;
    bool sprinting= false;
    Vector3 lastMove = new Vector3();
    [SyncVar]
    float attackTurn = 0;
    [SyncVar]
    float abil1CD = 0;
    [SyncVar]
    float abil2CD = 0;
    [SyncVar]
    public int team;
    void FixedUpdate () {
        
        if (isServer)
        {
            buffholder.computeStats();
            if (registerHit)
            {
                registerHit = false;
                if (attack)
                {

                    RpcEndAttack();
                }
                if (hp.change(-hitDamage))
                {
                    Destroy(gameObject);
                }
                else
                {
                    RpcTakeHit(hitDirection * hitMag);
                }
                
            }
            if (abil1CD > 0)
            {
                abil1CD -= Time.fixedDeltaTime;
            }
            if (abil2CD > 0)
            {
                abil2CD -= Time.fixedDeltaTime;
            }

        }
        if (hasAuthority)
        {
            buffholder.computeStats();
            ghost.renderCD(abil1CD, abil2CD);
            bool movement = true;
            Vector3 move = new Vector3();
            switch (current)
            {
                #region states
                case PState.Free:
                    if (!grounded)
                    {
                        current = PState.Air;
                        movement = false;
                    }
                    if (inp.jump) //here
                    {
                        rb.AddForce(Vector3.up * jumpForce);
                        current = PState.Air;
                    }
                    break;
                case PState.Air:
                    if (grounded)
                    {
                        current = PState.Free;
                    }
                    else
                    {
                        movement = false;
                    }
                    break;
                case PState.KB:
                    if (planeVel.magnitude <= KBRegain)
                    {
                        current = PState.Free;
                    }
                    //print(planeVel);
                    movement = false;
                    planeVel = planeVel.normalized * (planeVel.magnitude - KBDeteriorate * Time.fixedDeltaTime * (grounded ? 1 : 0.4f));
                    break;
                 #endregion
            }
            if (movement)
            {
                #region movement
                
                if (inp.up)
                {
                    move += new Vector3(0, 0, 1);
                }
                if (inp.down)
                {
                    move += new Vector3(0, 0, -1);
                }
                if (inp.left)
                {
                    move += new Vector3(-1, 0, 0);
                }
                if (inp.right)
                {
                    move += new Vector3(1, 0, 0);
                }
                move.Normalize();
                //rb.AddForce(move * moveForce * Time.fixedDeltaTime);
                //if(planeVel.magnitude >= maxSpeed)
                //{
                //    planeVel = planeVel.normalized * maxSpeed;
                //}
                
                if (!sprinting )
                {
                    #region sprintS
                    if (move != Vector3.zero)
                    {
                        if (lastMove == move)
                        {
                            sprintBuildCurrent += Time.fixedDeltaTime;
                        }
                        else
                        {
                            float diff = (move - lastMove).magnitude;
                            if (diff > 1) { diff = 1; }
                            Mathf.Pow(diff, 3);
                            //print(diff);
                            sprintBuildCurrent -= sprintChangeLoss*diff;
                            if (sprintBuildCurrent < 0)
                            {
                                sprintBuildCurrent = 0;
                            }
                        }
                    }
                   
                    lastMove = move;
                    
                    if (sprintBuildCurrent > sprintBuild)
                    {
                        //print("sprinting");
                        setSprint(true);
                    }
                    else
                    {
                        vis.CmdSprintAlpha(sprintPer);
                    }
                    #endregion

                }
                planeVel = ghost.spawnMutate(move) * maxSpeed*((sprintSpeed)+1);
                #endregion
            }

            Vector3 dif;
            switch (look)
            {


                case LookState.Free:
                    dif = inp.target - transform.position;
                    dif.y = 0;
                    transform.rotation = Quaternion.LookRotation(dif);
                    break;
                case LookState.Attacking:
                    if(current!= PState.Air)
                    {
                        dif = inp.target - transform.position;
                        dif.y = 0;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(dif),attackTurn*Time.fixedDeltaTime);
                    }
                    break;

            }
            if (look == LookState.Free && inp.attacking)
            {
                if (buffholder.buf("FullSprint") < 1)
                {
                    setSprint(false);
                }
                if (inp.atk1)
                {
                    look = LookState.Attacking;
                    CmdAtk(attackType.basic, inp.groundTarget);
                }
                else if (inp.atk2)
                {
                    look = LookState.Attacking;
                    CmdAtk(attackType.heavy, inp.groundTarget);
                }
                else if (inp.abil1 && abil1CD<=0)
                {
                    look = LookState.Attacking;
                    CmdAtk(attackType.abil1, inp.groundTarget);
                }
                else if (inp.abil2&& abil2CD <=0)
                {
                    look = LookState.Attacking;
                    
                    CmdAtk(attackType.abil2, inp.groundTarget);
                }


                

            }
            vis.CmdPropagate();
        }


    }

    public void setSprint(bool s)
    {
        sprinting = s;
        //Debug.Log(s);
        if (s)
        {
            sprintBuildCurrent = sprintBuild;
        }
        else
        {
            sprintBuildCurrent = 0;
        }
        vis.CmdSprintAlpha(sprintPer);
    }
    float sprintPer
    {
        get
        {
            return sprintBuildCurrent / sprintBuild;
        }
    }
    float sprintSpeed
    {
        get
        {
            return sprinting ? sprintModFull : sprintModHalf * sprintPer;
        }
    }


    float hitMag;
    Vector3 hitDirection;
    int hitDamage;
    [SyncVar]
    int lastHit = -1;
    bool registerHit = false;
    [ClientRpc]
    void RpcTakeHit(Vector3 vel)
    {
        if (Mathf.Abs(vel.y) <= 0.001)
        {
            planeVel = vel;
        }
        else
        {
            rb.velocity = vel;
        }
        
        current = PState.KB;
        setSprint(false);
    }


    [ClientRpc]
    public void RpcEndAttack()
    {
        Destroy(attack);
        vis.CmdColorNose(false);
        attack = null;
        look = LookState.Free;
    }

    [Server]
    public void getHit(float force, Vector3 location, int dmg, int ownerTeam)
    {
        //GameObject hitter = NetworkServer.FindLocalObject(owner);
        //NetworkIdentity iden = hitter.GetComponent<NetworkIdentity>();
        //NetworkConnection nc = iden.connectionToClient;
        lastHit = ownerTeam;
        getHit(force, location, dmg);
    }
    [Server]
    public void getHit(float force, Vector3 dir, int dmg)
    {
        registerHit = true;
        hitMag = force;
        hitDirection = dir.normalized;
        hitDamage = dmg;
    }
    Vector3 planeVel
    {
        get
        {
           return new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        set
        {
            rb.velocity = new Vector3(value.x, rb.velocity.y, value.z);
        }
    }
    public bool grounded
    {
        get
        {
            RaycastHit r = new RaycastHit();
            
            return Physics.SphereCast(transform.position, col.radius, -transform.up, out r, col.bounds.extents.y + 0.03f, 1 << 9) 
                && rb.velocity.y<= 0.1f;
        }
    }
    void OnDestroy()
    {
        if (ghost)
        {
            ghost.charDied(lastHit);
        }
        
    }
    [Command]
    void CmdAtk(attackType a, Vector3 groundTarget)
    {
        //Debug.Log(atkPre);
        vis.RpcColorNose(true);
        GameObject atkPre = lookup(a);
        attack = Instantiate(atkPre, transform);
        Attack atk = attack.GetComponent<Attack>();
        atk.setOwner(netId, team);
        setCD(a, atk.cooldown);
        if (attack.GetComponent<AttackM>())
        {
            attack.GetComponent<AttackM>().setColliders(false);
            attackTurn = attack.GetComponent<AttackM>().turnDeg;
        }
        else if (attack.GetComponent<AttackS>())
        {
            AttackS spwn = attack.GetComponent<AttackS>();
            Vector3 tar= groundTarget;
            tar.y = 0;
            Vector3 loc = transform.position;
            loc.y = 0;
            if ((tar - loc).magnitude > spwn.range)
            {
                tar = (tar - loc).normalized * spwn.range + loc;
            }
            tar.y = groundTarget.y;
            spwn.target = tar;
            attackTurn = 0;
        }
        else
        {
            attackTurn = 360;
        }
        
        NetworkServer.Spawn(attack);
        
        
    }
    [Server]
    public void healthTeam()
    {
        hp = GetComponent<Health>();
        hp.setTeam(team);

        //Debug.Log(team);
    }
}
