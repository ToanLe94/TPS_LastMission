using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UserInput : MonoBehaviour
{
	public CharacterMovement characterMove { get; protected set; }
	public WeaponHandler weaponHandler { get; protected set; }
    Animator animator;

    [System.Serializable]
    public class InputSettings
    {
        public string verticalAxis = "Vertical";
        public string horizontalAxis = "Horizontal";
        public string jumpButton = "Jump";
        public string reloadButton = "Reload";
        public string aimButton = "Fire2";
        public string fireButton = "Fire1";
        public string dropWeaponButton = "DropWeapon";
        public string switchWeaponButton = "SwitchWeapon";
    }
    [SerializeField]
    public InputSettings input;

    [System.Serializable]
    public class OtherSettings
    {
        public float lookSpeed = 5.0f;
        public float lookDistance = 30.0f;
        public bool requireInputForTurn = true;
        //public LayerMask aimDetectionLayers;
    }
    [SerializeField]
    public OtherSettings other;

    float forward;
    public Camera mainCamera;

    public bool debugAim;
    public Transform spine;
    bool aiming;

    bool isMaxSpeed;
    //System.Func<bool> isRunningCombo2;
    //System.Func<bool> isRunningCombo3;

    Dictionary<Weapon, GameObject> crosshairPrefabMap = new Dictionary<Weapon, GameObject>();

    // Use this for initialization
    void Start()
    {
        characterMove = GetComponent<CharacterMovement>();
        weaponHandler = GetComponent<WeaponHandler>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        SetupCrosshairs ();
        //isRunningCombo2 += IsRunningCombo2;
        //isRunningCombo3 += IsRunningCombo3;
    }

    private bool IsRunningCombo2()
    {
        return weaponHandler.isAnimationRunning("RightPunching");
    }
    private bool IsRunningCombo3()
    {
        return weaponHandler.isAnimationRunning("Mma Kick");
    }
    void SetupCrosshairs () {
		if (weaponHandler.weaponsList.Count > 0)
        {
			foreach (Weapon wep in weaponHandler.weaponsList)
            {
				GameObject prefab = wep.weaponSettings.crosshairPrefab;
				if (prefab != null)
                {
					GameObject clone = (GameObject)Instantiate (prefab);
					crosshairPrefabMap.Add (wep, clone);
					ToggleCrosshair (false, wep);
				}
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
        CharacterLogic();
        CameraLookLogic();
        WeaponLogic();
        KungFuLogic();

    }

    void LateUpdate()
    {
        if (weaponHandler)
        {
            if (weaponHandler.currentWeapon)
            {
                if (aiming)
                    RotateSpine();
            }
        }
    }
    void KungFuLogic()
    {
        if (!weaponHandler.currentWeapon)
        {
            weaponHandler.Defense(Input.GetButton(input.aimButton));
            if (Input.GetButtonDown(input.fireButton))
            {
                if (weaponHandler.GetCombo() == 0)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (animator.GetFloat("Forward") == 0)
                        {
                            weaponHandler.SetCombo(4);

                        }
                        else if (animator.GetFloat("Forward") > 0 && animator.GetFloat("Forward") < 1)
                        {
                            weaponHandler.SetCombo(5);

                        }
                        else if (animator.GetFloat("Forward") == 1f)
                        {
                            weaponHandler.SetCombo(6);

                        }

                    }
                    else
                    {
                        weaponHandler.SetCombo(1);
                        //StartCoroutine("ResetCombo1");

                    }


                }
                else if (weaponHandler.GetCombo() == 1)
                {
                    if (weaponHandler.isAnimationRunning("LeftPunching"))
                    {
                        weaponHandler.SetCombo(2);

                    }
                   
                    //weaponHandler.SetCombo(2);
                    //StartCoroutine("ResetCombo2");



                }
                else if (weaponHandler.GetCombo() == 2)
                {
                    if (weaponHandler.isAnimationRunning("RightPunching"))
                    {
                        weaponHandler.SetCombo(3);
                        //StartCoroutine("ResetCombo3");

                    }
                   
                }

            }
            else
            {
                if (weaponHandler.GetCombo() == 1 && !weaponHandler.isAnimationRunning("LeftPunching"))
                {
                    weaponHandler.SetCombo(0);

                }
                if (weaponHandler.GetCombo() == 2 && !weaponHandler.isAnimationRunning("RightPunching") && !weaponHandler.isAnimationRunning("LeftPunching"))
                {
                    weaponHandler.SetCombo(0);

                }
                if (weaponHandler.GetCombo() == 3 && !weaponHandler.isAnimationRunning("Mma Kick") && !weaponHandler.isAnimationRunning("RightPunching"))
                {
                    weaponHandler.SetCombo(0);

                }
                if ( weaponHandler.GetCombo() == 4|| weaponHandler.GetCombo() == 5 || weaponHandler.GetCombo() == 6)
                {
                    weaponHandler.SetCombo(0);

                }


            }
           
        }

    }

    //IEnumerator ResetCombo1()
    //{
    //    yield return new WaitForSeconds (0.5f);
    //    if (weaponHandler.GetCombo()==1)
    //    {
    //        weaponHandler.SetCombo(0);
    //    }
        

    //}
    //IEnumerator ResetCombo2()
    //{
    //    yield return new WaitUntil(isRunningCombo2);
    //    Debug.Log("Bat dau reset combo222222222");
    //    yield return new WaitForSeconds(0.46f);
    //    if (weaponHandler.GetCombo() == 2)
    //    {
    //        weaponHandler.SetCombo(0);
    //    }
        
        

    //}
    //IEnumerator ResetCombo3()
    //{
    //    Debug.Log("vao reset com bo 3");
    //    yield return new WaitUntil(isRunningCombo3);
    //    Debug.Log("Bat dau reset combo333333333");
    //    weaponHandler.SetCombo(0);
        



    //}
    //Handles character logic
    void CharacterLogic()
    {
        if (!characterMove)
            return;
        if (Input.GetKey(KeyCode.W) == true)
        {
            if (isMaxSpeed == false)
            {
                forward += Time.deltaTime * 3;
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    if (forward >= 0.5f)
                    {
                        if (forward >= 1)
                        {
                            isMaxSpeed = true;
                        }
                        else
                        {
                            forward = 0.5f;

                        }
                    }

                }
                else
                {
                    if (forward >= 1.0f)
                    {
                        forward = 1.0f;
                    }
                }
            }
            else
            {
                forward -= Time.deltaTime * 3;
                if (forward <= 0.5)
                {
                    forward = 0.5f;
                    isMaxSpeed = false;
                }
            }

    

        }
        else if (Input.GetKey(KeyCode.S) == true)
        {
            forward -= Time.deltaTime * 3;
            if (forward <= -0.5f)
            {
                forward = -0.5f;
            }

           
        }
        else
        {
            forward -= Time.deltaTime * 3;
            if (forward <= 0)
            {
                forward = 0;
            }
        }

      
        characterMove.Animate(forward, Input.GetAxis(input.horizontalAxis));

        if (Input.GetButtonDown(input.jumpButton) && (weaponHandler.GetCombo()!=4 && weaponHandler.GetCombo() != 5 && weaponHandler.GetCombo() != 6))
            characterMove.Jump();
        
        //characterMove.AirControl(Input.GetAxis(input.verticalAxis), Input.GetAxis(input.horizontalAxis));



    }

    //Handles camera logic
    void CameraLookLogic()
    {
        if (!mainCamera)
            return;
		
		other.requireInputForTurn = !aiming;

		if (other.requireInputForTurn) {
			if (Input.GetAxis (input.horizontalAxis) != 0 || Input.GetAxis (input.verticalAxis) != 0) {
                PlayerLook();

                //characterMove.CharacterLook();
			}
		}
		else {
            PlayerLook();
            //characterMove.CharacterLook();

        }
    }

    //Handles all weapon logic
    void WeaponLogic()
    {
        if (!weaponHandler)
            return;

		aiming = Input.GetButton (input.aimButton) || debugAim;

		weaponHandler.Aim (aiming);

		if (Input.GetButtonDown (input.switchWeaponButton))
        {
			weaponHandler.SwitchWeapons ();
			UpdateCrosshairs ();
		}
		
		if (weaponHandler.currentWeapon)
        {
			
			Ray aimRay = new Ray (mainCamera.transform.position, mainCamera.transform.forward);

			//Debug.DrawRay (aimRay.origin, aimRay.direction);
			if (Input.GetButton (input.fireButton) && aiming)
				weaponHandler.FireCurrentWeapon (aimRay);
			if (Input.GetButtonDown (input.reloadButton))
				weaponHandler.Reload ();
			if (Input.GetButtonDown (input.dropWeaponButton))
            {
                DeleteCrosshair(weaponHandler.currentWeapon);
                weaponHandler.DropCurWeapon();
			}

            if (weaponHandler.currentWeapon)
            {
                if (aiming)
                {
                    ToggleCrosshair(true, weaponHandler.currentWeapon);
                    PositionCrosshair(aimRay, weaponHandler.currentWeapon);
                }
                else
                    ToggleCrosshair(false, weaponHandler.currentWeapon);
            }
			
		} else
			TurnOffAllCrosshairs ();
    }

	public void TurnOffAllCrosshairs () {
       
            foreach (Weapon wep in crosshairPrefabMap.Keys)
            {
                ToggleCrosshair(false, wep);
            }
            
		
	}

	void CreateCrosshair (Weapon wep)
    {
		GameObject prefab = wep.weaponSettings.crosshairPrefab;
		if (prefab != null) {
			prefab = Instantiate (prefab);
			ToggleCrosshair (false, wep);
		}
	}
    
	void DeleteCrosshair (Weapon wep)
    {
		if (!crosshairPrefabMap.ContainsKey (wep))
			return;

		Destroy (crosshairPrefabMap [wep]);
		crosshairPrefabMap.Remove (wep);
	}

	// Position the crosshair to the point that we are aiming
	void PositionCrosshair (Ray ray, Weapon wep)
	{
		Weapon curWeapon = weaponHandler.currentWeapon;
		if (curWeapon == null)
			return;
		if (!crosshairPrefabMap.ContainsKey (wep))
			return;

		GameObject crosshairPrefab = crosshairPrefabMap [wep];
		RaycastHit hit;
		Transform bSpawn = curWeapon.weaponSettings.bulletSpawn;
		Vector3 bSpawnPoint = bSpawn.position;
		Vector3 dir = ray.GetPoint(curWeapon.weaponSettings.range) - bSpawnPoint;

		if (Physics.Raycast (bSpawnPoint, dir, out hit, curWeapon.weaponSettings.range, 
			curWeapon.weaponSettings.bulletLayers)) {
			if (crosshairPrefab != null) {
				ToggleCrosshair (true, curWeapon);
				crosshairPrefab.transform.position = hit.point;
				crosshairPrefab.transform.LookAt (Camera.main.transform);
			}
		} else {
			ToggleCrosshair (false, curWeapon);
		}
	}

	// Toggle on and off the crosshair prefab
	void ToggleCrosshair(bool enabled, Weapon wep)
	{
		if (!crosshairPrefabMap.ContainsKey(wep))
			return;

		crosshairPrefabMap [wep].SetActive (enabled);
	}

	void UpdateCrosshairs () {
		if (weaponHandler.weaponsList.Count == 0)
			return;

		foreach (Weapon wep in weaponHandler.weaponsList) {
			if (wep != weaponHandler.currentWeapon) {
				ToggleCrosshair (false, wep);
			}
		}
	}

    //Postions the spine when aiming
    void RotateSpine()
    {
        if (!spine || !weaponHandler.currentWeapon || !mainCamera)
            return;

        Transform mainCamT = mainCamera.transform;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 dir = mainCamT.forward;
        Ray ray = new Ray(mainCamPos, dir);

        spine.LookAt(ray.GetPoint(50));

        Vector3 eulerAngleOffset = weaponHandler.currentWeapon.userSettings.spineRotation;
        spine.Rotate(eulerAngleOffset);
    }

    //Make the character look at a forward point from the camera
    void PlayerLook()
    {
        Transform mainCamT = mainCamera.transform;
        Transform pivotT = mainCamT.parent;
        Vector3 pivotPos = pivotT.position;
        Vector3 lookTarget = pivotPos + (pivotT.forward * other.lookDistance);
        Vector3 thisPos = transform.position;
        Vector3 lookDir = lookTarget - thisPos;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        lookRot.x = 0;
        lookRot.z = 0;

        Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * other.lookSpeed);
        transform.rotation = newRotation;
    }
}
