using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillReward : MonoBehaviour {
	public int exp;
	public int fund;

    public FundSystem fundSystem { get { return FindObjectOfType<FundSystem>(); } set { fundSystem = value; } }
    public LevelSystem levelSystem { get { return FindObjectOfType<LevelSystem>(); } set { levelSystem = value; } }
    public RewardText rewardText { get { return FindObjectOfType<RewardText>(); } set { rewardText = value; } }
    private CharacterStats zombieStat { get { return GetComponent<CharacterStats>(); } set { zombieStat = value; } }

    private void Start()
    {
            
    }
    private void Update()
    {
        if (zombieStat.IsDead)
        {
            GetKillReward();
            rewardText.Show(exp, fund);
            this.enabled = false;
        }
    }
    public void GetKillReward()
    {
        levelSystem.GiveExp(exp);
        fundSystem.AddFund(fund);
    }
}
