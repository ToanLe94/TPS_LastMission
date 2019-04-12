using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player_NW : NetworkBehaviour {
    UserInput_NW userInput;
    CharacterMovement charMove;
    WeaponHandler weaponHandler;
    CameraRig_NW cameraRig;

    public static GameObject localPlayerObj;
    public Vector3 positionPlayer1 = new Vector3(-0.07f, 0.612f, 0.4199576f);
    public Vector3 positionPlayer2 = new Vector3(-2.54f, 0.6861355f, -1.22f);

    public static int currentPlayer_number = 0;

    [SyncVar(hook = "OnPositionPlayer")]
    public int player_number;


    [SyncVar]
    Vector3 syncPos;
    [SyncVar]
    Quaternion syncRot;


    [System.Serializable]
    public class MovementSettings
    {
        public Transform thisTransform;
        public float lerpRate = 15.0f;
    }
    [SerializeField]
    public MovementSettings movement;


    public override void OnStartLocalPlayer()
    {
        localPlayerObj = this.gameObject;
        CmdPosition_SpawnPlayer();
      
    }

    private void FixedUpdate()
    {
        #region Sync position and rotation.
        TransmitPosition();
        LerpPos();
        TransmitRotation();
        LerpRot();
        #endregion
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
            switch (player_number)
            {
                case 0:
                    //Debug.Log("postion 1" + positionPlayer1);
                    localPlayerObj.transform.position = positionPlayer1;
                    //Debug.Log("tranform localPlayerObj" + localPlayerObj.transform.position);

                    break;
                case 1:
                    //Debug.Log("postion 2" + positionPlayer2);
                    localPlayerObj.transform.position = positionPlayer2;
                    //Debug.Log("tranform localPlayerObj" + localPlayerObj.transform.position);

                    break;

                default:
                    break;
            }
            UpdatePostition(localPlayerObj.transform.position);
            UpdateRotation(localPlayerObj.transform.rotation);
        }

    }
    
    private void Update()
    {

        if (isLocalPlayer)
        {

        }
    }

    private void Start()
    {
        userInput = GetComponent<UserInput_NW>();
        charMove = GetComponent<CharacterMovement>();
        weaponHandler = GetComponent<WeaponHandler>();
        cameraRig = GetComponentInChildren<CameraRig_NW>();
        if (!isLocalPlayer)
        {
            userInput.enabled = false;
            GameObject camRigObj = cameraRig.gameObject;
            camRigObj.SetActive(false);

        }
        else
        {
            userInput.enabled = true;
            GameObject camRigObj = cameraRig.gameObject;
            camRigObj.SetActive(true);
            camRigObj.transform.SetParent(null);
        }
        //Debug.Log("Vao Start");
        //CmdPosition_SpawnPlayer();
    }
    #region Update tranform
    public static void UpdatePostition(Vector3 Pos)
    {
        localPlayerObj.GetComponent<Player_NW>().CmdUpdatePosition(Pos);
    }
    [Command]
    private void CmdUpdatePosition(Vector3 Pos)
    {

        RpcUpdatePosition(Pos);
        
    }
    [ClientRpc]
    private void RpcUpdatePosition(Vector3 Pos)
    {
        if (!isLocalPlayer)
        {
            this.transform.position = Pos;
        }
      
    }

    public static void UpdateRotation(Quaternion Rot)
    {
        localPlayerObj.GetComponent<Player_NW>().CmdUpdateRotation(Rot);
    }
    [Command]
    private void CmdUpdateRotation(Quaternion Rot)
    {

        RpcUpdateRotation(Rot);

    }
    [ClientRpc]
    private void RpcUpdateRotation(Quaternion Rot)
    {
        if (!isLocalPlayer)
        {
            this.transform.rotation = Rot;
        }

    }
    #endregion
    public static void Movement(float forward, float strafe)
    {

        localPlayerObj.GetComponent<Player_NW>().CmdMovement(forward, strafe);
    }

    [Command]
    private void CmdMovement(float forward,float strafe)
    {
        RpcMovement(forward, strafe);

    }
    [ClientRpc]
    private void RpcMovement(float forward, float strafe)
    {
        if (!isLocalPlayer)
        {
            GetComponent<CharacterMovement>().Animate(forward, strafe);
        }
    }
   public static void Jump()
    {
        localPlayerObj.GetComponent<Player_NW>().CmdJump();
    }

    [Command]
    private void CmdJump()
    {
        RpcJump();
    }


    [ClientRpc]
    private void RpcJump()
    {
        if (!isLocalPlayer)
        {
            GetComponent<CharacterMovement>().Jump();
            
        }
    }

   
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

}
