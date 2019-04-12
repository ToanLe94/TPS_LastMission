using UnityEngine;
using System.Collections;

public class GameController_NW : MonoBehaviour
{

    public static GameController_NW GC;

    private UserInput_NW player { get { return FindObjectOfType<UserInput_NW>(); } set { player = value; } }

    private PlayerUI playerUI { get { return FindObjectOfType<PlayerUI>(); } set { playerUI = value; } }

    private WeaponHandler wp { get { return player.GetComponent<WeaponHandler>(); } set { wp = value; } }

    private CharacterStats charStats { get { return player.GetComponent<CharacterStats>(); } set { charStats = value; } }

    void Awake()
    {
        if (GC == null)
        {
            GC = this;
        }
        else
        {
            if (GC != this)
            {
                Destroy(gameObject);
            }
        }
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (player && player.isLocalPlayer)
        {
            Debug.Log("Vao update UIPLayer local player");
            if (playerUI)
            {
                if (wp)
                {
                    if (playerUI.ammoText)
                    {
                        if (wp.currentWeapon == null)
                        {
                            playerUI.ammoText.text = "Unarmed.";
                        }
                        else
                        {
                            playerUI.ammoText.text = wp.currentWeapon.ammo.clipAmmo + "//" + wp.currentWeapon.ammo.carryingAmmo;
                        }
                    }
                }
                if (playerUI.healthBar && playerUI.healthText)
                {
                    Debug.Log("healthbar value :" + playerUI.healthBar.value + "health cua player: " + charStats.health);
                    playerUI.healthBar.value = charStats.health;

                    playerUI.healthText.text = Mathf.Round(playerUI.healthBar.value).ToString();
                }
            }
        }
    }
}
