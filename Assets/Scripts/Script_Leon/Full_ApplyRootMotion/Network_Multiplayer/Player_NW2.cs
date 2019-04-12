using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Player_NW2 : NetworkBehaviour {
    //UserInput uInput;
    UserInput_NW uInput;
    CharacterMovement charMove;
    WeaponHandler weaponHandler;
    CameraRig cameraRig;
    Animator animator;
    CharacterStats charStats;

    public static Player_NW2 localPlayerObj;

    [System.Serializable]
    public class MovementSettings
    {
        public Transform thisTransform;
        public float lerpRate = 15.0f;
    }
    [SerializeField]
    public MovementSettings movement;

  

    public Transform Spine_Transform;

    ////public GameObject startWeapon;

    //public bool setStartWeapon;

    #region Sync variables.
    [SyncVar]
    Vector3 syncPos;
    [SyncVar]
    Quaternion syncRot;

    #region Animation
    [SyncVar]
    bool syncGrounded;
    [SyncVar]
    float syncForward;
    [SyncVar]
    float syncStrafe;
    [SyncVar]
    bool syncJump;
    [SyncVar]
    int syncWeaponType;
    [SyncVar]
    bool syncReloading;
    [SyncVar]
    bool syncAiming;

    [SyncVar]
    Quaternion syncRotSpine;

    [SyncVar]
    Vector3 syncPosCamera;
    [SyncVar]
    Quaternion syncRotCamera;
    #endregion

    [SyncVar]
    GameObject syncStartWeapon;

    [SyncVar]
    float syncHealth;
    #endregion

    public static int currentPlayer_number = 0;

    [SyncVar(hook = "OnPositionPlayer")]
    public int player_number;

    public override void OnStartLocalPlayer()
    {
        localPlayerObj = this;
        CmdPosition_SpawnPlayer();

    }

    [Command]
    private void CmdPosition_SpawnPlayer()
    {
        player_number = currentPlayer_number;
        currentPlayer_number++;

    }
    private void OnPositionPlayer(int newnumber)
    {

        player_number = newnumber;

        if (isLocalPlayer)
        {
            //switch (player_number)
            //{
            //    case 0:
            //        //Debug.Log("postion 1" + positionPlayer1);
            //        localPlayerObj.transform.position = positionPlayer1;
            //        //Debug.Log("tranform localPlayerObj" + localPlayerObj.transform.position);

            //        break;
            //    case 1:
            //        //Debug.Log("postion 2" + positionPlayer2);
            //        localPlayerObj.transform.position = positionPlayer2;
            //        //Debug.Log("tranform localPlayerObj" + localPlayerObj.transform.position);

            //        break;

            //    default:
            //        break;
            //}
            //UpdatePostition(localPlayerObj.transform.position);
            //UpdateRotation(localPlayerObj.transform.rotation);
        }

    }
    // Use this for initialization
    void Start()
    {

        #region Assing components.
        //uInput = GetComponent<UserInput>();
        uInput = GetComponent<UserInput_NW>();
        charMove = GetComponent<CharacterMovement>();
        weaponHandler = GetComponent<WeaponHandler>();
        cameraRig = GetComponentInChildren<CameraRig>();
        animator = GetComponent<Animator>();
        charStats = GetComponent<CharacterStats>();
        #endregion

        #region Setup player.
        if (isLocalPlayer)
        {
            uInput.enabled = true;
            GameObject camRigObj = cameraRig.gameObject;
            camRigObj.SetActive(true);
            camRigObj.transform.SetParent(null);
        }
        else
        {
            uInput.enabled = false;
            GameObject camRigObj = cameraRig.gameObject;
            camRigObj.SetActive(false);
        }
        #endregion

        #region Spawn player.
        if (FindObjectOfType<SpawnPoint_NW>())
        {
            SpawnPoint_NW netSpawn = FindObjectOfType<SpawnPoint_NW>();
            Transform desiredSpawn = netSpawn.spawnPoints[Random.Range(0, netSpawn.spawnPoints.Length)];
            transform.position = desiredSpawn.position;
        }
        #endregion

        //CreateStartWeapon();
        //StartCoroutine(WaitForAlittleBit());
    }

    //IEnumerator WaitForAlittleBit()
    //{
    //    yield return new WaitForSeconds(2);
    //    SetStartWeapon();
    //}
    private void LateUpdate()
    {
        TransmitRotSpine();
        SetRotSpine();
    }
    void Update()
    {
        TransmitAnimations();
        SetAnimations();
        //TransmitHealth();
        //UpdateHealth();

        #region Sync position and rotation player.
        TransmitPosition();
        LerpPos();
        TransmitRotation();
        LerpRot();
        #endregion

        //sync position and rot Camera on Client because when player aiming and RotateSpine onLocalPlayer, All client turn off userInput so that function won't run. 
        //So we have to Update Pos and Rot of Camera on All client
        TransmitTranformCamera();
        UpdateTranformCamera();
    }

    void FixedUpdate()
    {
      

       
    }

    void LerpPos()
    {
        if (!isLocalPlayer)
        {
            movement.thisTransform.position = Vector3.Lerp(movement.thisTransform.position, syncPos, Time.deltaTime * movement.lerpRate);
        }
    }

    void LerpRot()
    {
        if (!isLocalPlayer)
        {
            movement.thisTransform.rotation = Quaternion.Lerp(movement.thisTransform.rotation, syncRot, Time.deltaTime * movement.lerpRate);
        }
    }

    void SetAnimations()
    {
        if (!isLocalPlayer)
        {
            animator.SetBool(charMove.animations.groundedBool, syncGrounded);
            animator.SetFloat(charMove.animations.verticalVelocityFloat, syncForward);
            animator.SetFloat(charMove.animations.horizontalVelocityFloat, syncStrafe);
            animator.SetBool(charMove.animations.jumpBool, syncJump);
            animator.SetInteger(weaponHandler.animations.weaponTypeInt, syncWeaponType);
            animator.SetBool(weaponHandler.animations.reloadingBool, syncReloading);
            animator.SetBool(weaponHandler.animations.aimingBool, syncAiming);
        }
    }
    void SetRotSpine()
    {
        if (!isLocalPlayer)
        {
            if (weaponHandler)
            {
                if (weaponHandler.currentWeapon)
                {
                    if (animator.GetBool(weaponHandler.animations.aimingBool))
                    {
                        Spine_Transform.rotation = syncRotSpine;

                    }
                }
            }
        }
    }
    //void SetStartWeapon()
    //{
    //    if (!isLocalPlayer)
    //    {
    //        weaponHandler.currentWeapon = syncStartWeapon.GetComponent<Weapon>();
    //    }
    //}

    [Command]
    void Cmd_PassPosition(Vector3 pos)
    {
        syncPos = pos;
    }

    [Command]
    void Cmd_PassRotation(Quaternion rot)
    {
        syncRot = rot;
    }

    [Command]
    void Cmd_PassAnimations(bool grounded, float forward, float strafe, bool jump, int wepType, bool reloading, bool aiming)
    {
        syncGrounded = grounded;
        syncForward = forward;
        syncStrafe = strafe;
        syncJump = jump;
        syncWeaponType = wepType;
        syncReloading = reloading;
        syncAiming = aiming;
    }
    [Command]
    void Cmd_PassRotSpine(Quaternion rot)
    {
        syncRotSpine = rot;
    }
    //[Command]
    //void Cmd_CreateWeapon(GameObject weapon)
    //{
    //    GameObject wep = (GameObject)Instantiate(Resources.Load("Network Pistol"), transform.position, transform.rotation);
    //    NetworkServer.Spawn(wep);
    //    syncStartWeapon = wep;
    //    weaponHandler.currentWeapon = wep.GetComponent<Weapon>();
    //}

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
        {
            Cmd_PassPosition(movement.thisTransform.position);
        }
    }
    
    [ClientCallback]
    void TransmitRotation()
    {
        if (isLocalPlayer)
        {
            Cmd_PassRotation(movement.thisTransform.rotation);
        }
    }
    [ClientCallback]
    void TransmitRotSpine()
    {
        if (isLocalPlayer)
        {
            if (weaponHandler)
            {
                if (weaponHandler.currentWeapon)
                {
                    if (animator.GetBool(weaponHandler.animations.aimingBool))
                    {
                        Cmd_PassRotSpine(Spine_Transform.rotation);

                    }
                }
            }

        }
    }

    [ClientCallback]
    void TransmitTranformCamera()
    {
        if (isLocalPlayer)
        {
            Cmd_PassTranformCamera(uInput.mainCamera.transform.position, uInput.mainCamera.transform.rotation);
        }
    }
    [Command]
    void Cmd_PassTranformCamera(Vector3 pos,Quaternion rot)
    {
        syncPosCamera = pos;
        syncRotCamera = rot;

    }
   


    void UpdateTranformCamera()
    {
        if (!isLocalPlayer)
        {
            uInput.mainCamera.transform.position = syncPosCamera;
            uInput.mainCamera.transform.rotation = syncRotCamera;
        }
    }

    [ClientCallback]
    void TransmitAnimations()
    {
        if (isLocalPlayer)
        {
            Cmd_PassAnimations(animator.GetBool(charMove.animations.groundedBool),
                animator.GetFloat(charMove.animations.verticalVelocityFloat),
                animator.GetFloat(charMove.animations.horizontalVelocityFloat),
                animator.GetBool(charMove.animations.jumpBool),
                animator.GetInteger(weaponHandler.animations.weaponTypeInt),
                animator.GetBool(weaponHandler.animations.reloadingBool),
                animator.GetBool(weaponHandler.animations.aimingBool));

        }
    }

    //[ClientCallback]
    //void CreateStartWeapon()
    //{
    //    if (isLocalPlayer)
    //    {
    //        Cmd_CreateWeapon(startWeapon);
    //    }
    //}


    //Command va clienrpc to dong bo thuc hien ham SwitchWeapon
    public static void SwitchWeapons()
    {
        localPlayerObj.CmdSwitchWeapons();
    }
    [Command]
    private void CmdSwitchWeapons ()
    {

        RpcSwitchWeapons();

    }
    [ClientRpc]
    private void RpcSwitchWeapons()
    {
        if (!isLocalPlayer)
        {
            weaponHandler.SwitchWeapons();

        }

    }

    public static void FireCurrentWeapon()
    {
        localPlayerObj.CmdFireCurrentWeapon();
    }
    [Command]
    private void CmdFireCurrentWeapon()
    {

        RpcFireCurrentWeapon();

    }
    [ClientRpc]
    private void RpcFireCurrentWeapon()
    {
        if (!isLocalPlayer)
        {
            Ray aimRay = new Ray(uInput.mainCamera.transform.position, uInput.mainCamera.transform.forward);
            //Debug.Log("Gia tri :" + aimRay.origin +" vaa: " + aimRay.direction);
            Debug.DrawRay(aimRay.origin, aimRay.direction, Color.red);
            weaponHandler.FireCurrentWeapon(aimRay);

        }

    }

    public static void Reload()
    {
        localPlayerObj.CmdReload();
    }
    [Command]
    private void CmdReload()
    {

        RpcReload();

    }
    [ClientRpc]
    private void RpcReload()
    {
        if (!isLocalPlayer)
        {
            weaponHandler.Reload();

        }

    }

    public static void DropCurWeapon()
    {
        localPlayerObj.CmdDropCurWeapon();
    }
    [Command]
    private void CmdDropCurWeapon()
    {

        RpcDropCurWeapon();

    }
    [ClientRpc]
    private void RpcDropCurWeapon()
    {
        if (!isLocalPlayer)
        {
            weaponHandler.DropCurWeapon();

        }

    }

    public  void Die()
    {
        CmdDie();
       
    }
    [Command]
    private void CmdDie()
    {

        RpcDie();

    }
    [ClientRpc]
    private void RpcDie()
    {      
         charStats.Die();
    }

    public void UpdateHealth(float damage)
    {
        
        //this.gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
        CmdUpdateHealth(damage);
    }
    [Command]
    private void CmdUpdateHealth( float damage)
    {

        RpcUpdateHealth(damage);

    }
    [ClientRpc]
    private void RpcUpdateHealth(float damage)
    {
        charStats.health -= damage;
    }

    [ClientCallback]
    void TransmitHealth()
    {
        Cmd_PassHealth(charStats.health);
    }
    [Command]
    void Cmd_PassHealth(float health)
    {
        syncHealth = health;

    }

    void UpdateHealth()
    {
        charStats.health = syncHealth;

    }

}
