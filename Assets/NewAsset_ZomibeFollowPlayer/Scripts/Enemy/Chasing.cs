using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chasing : MonoBehaviour {
	NetworkManager networkManager;
	Animator animator;
	CharacterStats healthManager;
	NavMeshAgent agent;
	AudioSource audioSource;
	public GameObject target;
	public float damage = 2.0f;
	public bool isAttacking = false;
	public bool shouldChase = true;
	public bool isInLateUpdate = false;
	public bool shouldUpdate = true;

	public AudioClip attackSound;
	public AudioClip deathSound;
    private PrefabManager prefabManager;
    // Use this for initialization
    void Start () {
		animator = GetComponent<Animator>();
		healthManager = GetComponent<CharacterStats>();
		agent = GetComponent<NavMeshAgent>();
		audioSource = GetComponent<AudioSource>();

		networkManager = GameObject.Find("GameManager").GetComponent<NetworkManager>();
        prefabManager = PrefabManager.GetInstance();

    }

	IEnumerator distUpdateCo = null;

	// Update is called once per frame
	void Update () {
		if(!shouldUpdate) return;

		if(!healthManager.IsDead) {
			if(!isAttacking) {
				// NetworkPlayer targetNetworkPlayer = target.GetComponent<NetworkPlayer>();

				// if(targetNetworkPlayer.IsLocalPlayer) {
					float distance = GetActualDistanceFromTarget();

					// Reduce calculation of path finding
					if(distance <= 20f) {
						if(distUpdateCo != null) {
							StopCoroutine(distUpdateCo);
						}

						isInLateUpdate = false;
						agent.destination = target.transform.position;
					}
					else if(!isInLateUpdate) {
						if(distance <= 40f) {
							distUpdateCo = LateDistanceUpdate(2f);
							StartCoroutine(distUpdateCo);
						}
						else if(distance <= 60) {
							distUpdateCo = LateDistanceUpdate(3f);
							StartCoroutine(distUpdateCo);
						}
						else if(distance <= 80) {
							distUpdateCo = LateDistanceUpdate(4f);
							StartCoroutine(distUpdateCo);
						}
						else {
							distUpdateCo = LateDistanceUpdate(5f);
							StartCoroutine(distUpdateCo);
						}
					}
				// }
			}

			animator.SetFloat("SpeedMultiplier", agent.speed);

			if(agent.pathPending) return;

			CheckAttack();
		}
		else {
			shouldUpdate = false;
			shouldChase = false;
			
			audioSource.PlayOneShot(deathSound);

			Destroy(agent);

			StartCoroutine(RemoveGameObject());
			return;
		}
	}

	IEnumerator LateDistanceUpdate(float duration) {
		isInLateUpdate = true;
		agent.destination = target.transform.position;
		yield return new WaitForSeconds(duration);
		
		isInLateUpdate = false;
		distUpdateCo = null;
		yield break;
	}

	float GetActualDistanceFromTarget() {
		return GetDistanceFrom(target.transform.position, this.transform.position);
	}

	float GetDistanceFrom(Vector3 src, Vector3 dist) {
		return Vector3.Distance(src, dist);
	}

	float origSpeed;

	void CheckAttack() {
		// Calculate actual distance from target
		float distanceFromTarget = GetActualDistanceFromTarget();
		
		// Calculate direction is toward player
		Vector3 direction = target.transform.position - this.transform.position;
		float angle = Vector3.Angle(direction, this.transform.forward);

		if(!isAttacking && distanceFromTarget <= 0.8f && angle <= 60f  && target.GetComponent<CharacterStats>().IsDead ==false) {
			isAttacking = true;
			shouldChase = false;

			origSpeed = agent.speed;
			agent.speed = 0;

			audioSource.PlayOneShot(attackSound);
			animator.SetTrigger("Attack");
            CreateBlood(target.transform.position + new Vector3(0, 0.5f, 0));
			CharacterStats targetHealthManager = target.GetComponent<CharacterStats>();

			if(targetHealthManager) {
				targetHealthManager.Damage(damage);
			}

			StartCoroutine(ResetAttacking());
		}
	}

	IEnumerator ResetAttacking() {
		yield return new WaitForSeconds(1.4f);

		isAttacking = false;
		shouldChase = true;

		if(!healthManager.IsDead) {
			agent.speed = origSpeed;
		}

		
		yield break;
	}
    void CreateBlood(Vector3 pos)
    {
        GameObject blood = prefabManager.GetPrefab("BloodEffect");
        GameObject bloodEffect = Instantiate(blood, pos, new Quaternion(0, 0, 0, 0));
        Destroy(bloodEffect, 3f);
    }
    IEnumerator RemoveGameObject() {
		yield return new WaitForSeconds(5f);
		// PhotonNetwork.Destroy(gameObject);
		Destroy(gameObject);
	}
}
