using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMover : NetworkBehaviour {
    public InputHandler inp;
    public GameObject attackPre1;
    public PlayerGhost ghost;

    Rigidbody rb;
    CapsuleCollider col;
    [SyncVar]
    PState current;
    [SyncVar]
    LookState look;
    GameObject attack;


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
        basic
    }

    GameObject lookup(attackType a)
    {
        switch (a)
        {
            case attackType.basic:
                return attackPre1;
            default:
                return attackPre1;
        }
            
    }
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        current = PState.Free;
        look = LookState.Free;
	}

    // Update is called once per frame
    //public float moveForce = 600;
    public float maxSpeed = 3;
    public float jumpForce = 20;
    public float KBDeteriorate = 2;
    public float KBRegain = 2;
    void FixedUpdate () {
        if (isServer)
        {
            if (registerHit)
            {
                registerHit = false;
                if (attack)
                {
                    endAttack();
                }
                takeHit(hitDirection * hitMag);
            }

            bool movement = true;
            switch (current)
            {
                #region states
                case PState.Free:
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
                    movement = false;
                    planeVel = planeVel.normalized * (planeVel.magnitude - KBDeteriorate * Time.fixedDeltaTime * (grounded ? 1 : 0.4f));
                    break;
                    #endregion
            }
            if (movement)
            {
                #region movement
                Vector3 move = new Vector3();
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

                planeVel = move * maxSpeed;
                #endregion
            }
            switch (look)
            {
                case LookState.Free:
                    Vector3 dif = inp.target - transform.position;
                    dif.y = 0;
                    transform.rotation = Quaternion.LookRotation(dif);
                    break;

            }
            if (look == LookState.Free && inp.atk1)
            {
                look = LookState.Attacking;

                atk(attackType.basic);

            }

        }
        if (isClient)
        {
            switch (look)
            {
                case LookState.Free:
                    Vector3 dif = inp.target - transform.position;
                    dif.y = 0;
                    transform.rotation = Quaternion.LookRotation(dif);
                    break;

            }


        }


    }
    float hitMag;
    Vector3 hitDirection;
    bool registerHit = false;
    [Server]
    void takeHit(Vector3 vel)
    {
        planeVel = vel;
        current = PState.KB;
    }


    [Server]
    public void endAttack()
    {
        Destroy(attack);
        attack = null;
        look = LookState.Free;
    }

    [Server]
    public void getHit(float force, Vector3 location)
    {
        registerHit = true;
        hitMag = force;
        hitDirection = transform.position - location;
        hitDirection.y = 0;
        hitDirection.Normalize();
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
    bool grounded
    {
        get
        {
            RaycastHit r = new RaycastHit();
            
            return Physics.SphereCast(transform.position, col.radius, -transform.up, out r, col.bounds.extents.y + 0.05f, 1 << 9) 
                && rb.velocity.y<= 0.1f;
        }
    }
    void OnDestroy()
    {
        if (ghost)
        {
            ghost.charDied();
        }
        
    }
    [Server]
    void atk(attackType a)
    {
        //Debug.Log(atkPre);
        GameObject atkPre = lookup(a);
        attack = Instantiate(atkPre, transform);
        attack.GetComponent<Attack>().setOwner(netId);
        NetworkServer.Spawn(attack);
        
        
    }

}
