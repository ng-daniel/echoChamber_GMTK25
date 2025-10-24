using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserParameters : MonoBehaviour
{
    [SerializeField] float rotateDegrees;
    [SerializeField] float rotateChargeTimeSec;
    [SerializeField] float rotateFireTimeSec;
    [SerializeField] float rotateWindDownTimeSec;
    [SerializeField] float windDownRotateDecrementSec;
    [SerializeField] int damage;

    public LaserParameters(
        float rotateDegrees,
        float rotateChargeTimeSec,
        float rotateFireTimeSec,
        float rotateWindDownTimeSec,
        float windDownRotateDecrementSec,
        int damage)
    {
        this.rotateDegrees = rotateDegrees;
        this.rotateChargeTimeSec = rotateChargeTimeSec;
        this.rotateFireTimeSec = rotateFireTimeSec;
        this.rotateWindDownTimeSec = rotateWindDownTimeSec;
        this.windDownRotateDecrementSec = windDownRotateDecrementSec;
        this.damage = damage;
    }

    public float GetRotateDegrees()
    {
        return rotateDegrees;
    }

    public float GetRotateChargeTimeSec()
    {
        return rotateChargeTimeSec;
    }
    public float GetRotateFireTimeSec()
    {
        return rotateFireTimeSec;
    }
    public float GetRotateWindDownTimeSec()
    {
        return rotateWindDownTimeSec;
    }
    public float GetWindDownRotateDecrementSec()
    {
        return windDownRotateDecrementSec;
    }
    public int GetDamage()
    {
        return damage;
    }

}
