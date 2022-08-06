using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{

    [SerializeField] EnergyBar energyBar;

    public const int MAX = 100;

    public const int PERCENT = 1;//每次击中敌人后加的能量值

    int energy;


    private void Start()
    {
        energyBar.Initialize(energy, MAX);
        Obtain(100);
    }


    public void Obtain(int value)
    {
        if (energy == MAX) return;

        //energy += value;
        //energy = Mathf.Clamp(energy, 0, MAX);

        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateStats(energy, MAX);

    }

    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateStats(energy, MAX);
    }

    public bool IsEnough(int value) => energy >= value;

}
