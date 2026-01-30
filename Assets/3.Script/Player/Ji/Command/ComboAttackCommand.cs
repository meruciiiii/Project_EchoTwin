using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttackCommand : IWeaponCommand
{
    private IWeaponCommand main;
    private IWeaponCommand sub;
    private MonoBehaviour coroutineRunner;

    public ComboAttackCommand(IWeaponCommand main, IWeaponCommand sub, MonoBehaviour runner)
    {
        this.main = main;
        this.sub = sub;
        this.coroutineRunner = runner;
    }

    public void execute()
    {
        main.execute();

        if(coroutineRunner != null && coroutineRunner.gameObject.activeInHierarchy)
        {
            coroutineRunner.StartCoroutine(SubAttack_co());
        }

    }

    private IEnumerator SubAttack_co()
    {
        yield return new WaitForSeconds(coroutineRunner.GetComponent<PlayerStats>().TimeBetweenAttack);

        sub?.execute();
    }
}
