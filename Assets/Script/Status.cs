using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;
    public float attackPower = 20f;
    public float guardPower = 20f;
    public float recoverPower = 20f;
    public float currentGuard;
    public float startGuard = 0;


    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        currentGuard = startGuard;
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
