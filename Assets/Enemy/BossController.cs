using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] GameObject laser;
    [SerializeField] Transform laserOriginPoint;
    [SerializeField] LaserParameters rotateLaserParam;

    public void LaserSpin()
    {
        GameObject newLaser = Instantiate(laser, laserOriginPoint.position, Quaternion.identity);
        LaserScript newLaserScript = newLaser.GetComponent<LaserScript>();
        newLaserScript.Initialize(this.gameObject, rotateLaserParam);
        newLaserScript.RotateAttackChain(Vector2.down);
    }

}
