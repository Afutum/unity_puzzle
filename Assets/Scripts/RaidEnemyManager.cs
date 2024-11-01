using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static EnemyManager;
using static UnityEngine.EventSystems.EventTrigger;

public class RaidEnemyManager : EnemyManager
{
    [SerializeField] GameObject[] enemyRandPrefab;

    public int raidId = 0;
    public int bossId = 0;
    int currentHp = 0;

    private void Start()
    {
        StartCoroutine(NetworkManager.Instance.GetRaidBosses(boss =>
        {
            if(boss != null && boss[0].NowHp > 0)
            {
                raidId = boss[0].Id;

                bossId = boss[0].BossId;
                currentHp = boss[0].NowHp;
            }
            else
            {
                bossId = UnityEngine.Random.Range(0, enemyRandPrefab.Length);
            }

            enemy = new GameObject[1];

            enemy[0] = Instantiate(enemyRandPrefab[bossId]);

            SetStart();
        }));
    }

    override protected void SetEnemyDamage()
    {
        hp = new int[1];
        baseDamege = new int[1];

        switch (bossId)
        {
            case 0:
                if (currentHp > 0)
                {
                    hp[0] = currentHp;
                }
                else
                {
                    hp[0] = 3000;
                }
                bossId = 1;
                baseDamege[0] = UnityEngine.Random.Range(300, 501);
                break;

            case 1:
                if (currentHp > 0)
                {
                    hp[0] = currentHp;
                }
                else
                {
                    hp[0] = 3000;
                }
                bossId = 2;
                baseDamege[0] = UnityEngine.Random.Range(350, 501);
                break;

            case 2:
                if (currentHp > 0)
                {
                    hp[0] = currentHp;
                }
                else
                {
                    hp[0] = 3200;
                }
                bossId = 3;
                baseDamege[0] = UnityEngine.Random.Range(400, 501);
                break;

            case 3:
                if (currentHp > 0)
                {
                    hp[0] = currentHp;
                }
                else
                {
                    hp[0] = 3500;
                }
                bossId = 4;
                baseDamege[0] = UnityEngine.Random.Range(450, 501);
                break;
        }
    }

    protected override void DiePlayer()
    {
        StartCoroutine(NetworkManager.Instance.RegistRaid(raidId, bossId, currentHp - hp[0],
                            result =>
                            {
                                gameDirector.LosePlayer();

                                gameDirector.GameEnd();
                            }));
    }

    override public void TerminateBattle()
    {
        StartCoroutine(NetworkManager.Instance.RegistRaid(raidId,bossId, currentHp - hp[0],
                            result =>
                            {
                                gameDirector.GameEnd();
                            }));
    }
}
