﻿using Interactive.Base;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : ChoicesSenderBehaviour
{
    [SerializeField] private GameObject missilePrefab;
    private Missile missile;
    [SerializeField] private House opponentHouse;
    [SerializeField] private float flightDuration = 5f;
    [SerializeField] private float height = 5f;
    [SerializeField] private int missileDamage = 5;
    [SerializeField] public float shakeAmplitude = 0.2f;
    [SerializeField] public float shakePeriod = 0.1f;
    [SerializeField] public float shakeDuration = 2;


    public void RechargeMissile()
    {
        if(missile == null)
        {
            missile = Instantiate(missilePrefab, transform).GetComponent<Missile>();
            missile.transform.position = transform.position;
            missile.Initialize(opponentHouse,flightDuration,height,missileDamage,shakeAmplitude,shakePeriod,shakeDuration);
        }
    }
    public void Fire()
    {
        if(missile != null)
        {
            missile.LaunchMissile();
        }
    }

    private void Upgrade()
    {
        Debug.Log("Upgrade");
    }

    public override List<GameAction> GetChoices(Player contextPlayer)
    {
        return new List<GameAction>() {
                new GameAction("Feu !", () => Fire(), () => true),
                new GameAction("Recharger", () => RechargeMissile(), () => true),
                new GameAction("Upgrade", () => Upgrade(), () => true)
            };
    }
}
