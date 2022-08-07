using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{

    [SerializeField] EnergyBar energyBar;

    [SerializeField]float overdriveInterval = 0.1f;

    bool available = true;

    public const int MAX = 100;

    public const int PERCENT = 1;//每次击中敌人后加的能量值

    int energy;

    WaitForSeconds waitForOverdriveInterval;

    protected override void Awake()
    {
        base.Awake();
        waitForOverdriveInterval = new WaitForSeconds(overdriveInterval);
        
    }

    private void OnEnable()
    {
        PlayerOverdrive.on += PlayerOverdriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    private void OnDisable()
    {
        PlayerOverdrive.on -= PlayerOverdriveOn;
        PlayerOverdrive.off -= PlayerOverdriveOff;
    }

    private void Start()
    {
        energyBar.Initialize(energy, MAX);
        Obtain(100);
    }


    public void Obtain(int value)
    {
        if (energy == MAX || !available || !gameObject.activeSelf) return;

        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateStats(energy, MAX);

    }

    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateStats(energy, MAX);

        if (energy == 0 && !available)
        {
            PlayerOverdrive.off.Invoke();
        }
    }

    public bool IsEnough(int value) => energy >= value;

    void PlayerOverdriveOn()
    {
        available = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }

    void PlayerOverdriveOff()
    {
        available = true;
        StopCoroutine(nameof(KeepUsingCoroutine));
    }


    IEnumerator KeepUsingCoroutine()
    {
        while (gameObject.activeSelf && energy > 0)
        {
            yield return waitForOverdriveInterval;


            // use 1% of max energy, every 1 second use 10% of max energy 每0.1秒消耗1点能量，1秒则消耗10点能量
            // means that overdrive last for 10 seconds   所以可以持续10秒
            Use(PERCENT);

        }
    }
}
