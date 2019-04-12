using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class CharacterStats_NW : NetworkBehaviour
{

    private CharacterController charactercontroller { get { return GetComponent<CharacterController>(); } set { charactercontroller = value; } }
    private RagdollManager ragdollManager { get { return GetComponentInChildren<RagdollManager>(); } set { ragdollManager = value; } }

    [SyncVar(hook = "UpdateChangeHealth")]
    [Range(0, 100)] public float health = 100;
    public int faction;
    public MonoBehaviour[] scriptsToDisable;
    public NetworkBehaviour[] scriptsNetWorkToDisable;
    public Player_NW2 player_NW { get { return GetComponent<Player_NW2>(); } set { player_NW = value; } }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, 100);

        //Debug.Log("O Update Player number :" + player_NW.player_number + "Gia tri health" + health);
    }

    public void Damage(float damage)
    {
        health -= damage;
        //Debug.Log("O ham Damage Player number :" + player_NW.player_number + "Gia tri health" + health);

        //player_NW.UpdateHealth(damage);
        if (health <= 0)
        {
            Die();
            //player_NW.Die();

        }
    }

    public void UpdateChangeHealth(float newvalue)
    {

        health = newvalue;
        Debug.Log(" o updateChangeHealth Player number: " + player_NW.player_number + "Gia tri health: " + health);

    }
    [Command]
    public void CmdDamage(float damage)
    {
        health -= damage;
        //player_NW.UpdateHealth(damage);
        if (health <= 0)
        {
            //Die();
            player_NW.Die();

        }
    }
    public void Die()
    {
        charactercontroller.enabled = false;
        if (scriptsToDisable.Length == 0)
        {
            Debug.Log("All scripts still working on this character but this is dead.");
            return;
        }
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            script.enabled = false;
        }

        if (scriptsNetWorkToDisable.Length != 0)
        {
            foreach (NetworkBehaviour script in scriptsNetWorkToDisable)
            {
                script.enabled = false;

            }
        }

        if (ragdollManager != null)
        {
            ragdollManager.Ragdoll();
        }
    }
}
