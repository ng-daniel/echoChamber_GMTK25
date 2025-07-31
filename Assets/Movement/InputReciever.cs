using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReciever : MonoBehaviour
{


    Rigidbody2D rb;

    List<InputData> inputs = new List<InputData>();

    [Header("MOVING")]
    [SerializeField] float speed;
    [SerializeField] float dashSpeed;
    bool isBusy;
    [Header("SHOOTING")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject bullet;
    [SerializeField] float fireIntervalSeconds;
    [SerializeField] float gunDistanceFromBody;
    float fireTimer;
    bool fireReady = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!fireReady)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer < fireIntervalSeconds)
            {
                fireTimer = 0;
                SetFireReady(true);
            }
        }
    }

    public void HandleInputs(InputData inputData)
    {
        if (isBusy) return;

        if (inputData.IsDashPress() && inputData.GetMoveDirection() != Vector2.zero)
        {
            Dash(inputData.GetMoveDirection());
            return;
        }

        if (inputData.IsMouseClick() && fireReady) FireBullet();

        AimGun(inputData.GetAimDirection());
        Move(inputData.GetMoveDirection());
    }

    void AimGun(Vector2 direction)
    {
        float angleDeg = Vector2.Angle(direction, Vector2.right);
        gun.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleDeg));
    }
    void Move(Vector2 direction)
    {
        rb.velocity = direction.normalized * speed;
    }

    void Dash(Vector2 direction)
    {
        rb.velocity = direction.normalized * dashSpeed;
    }
    void FireBullet()
    {
        SetFireReady(false);
    }
    void SetFireReady(bool val)
    {
        fireReady = val;
    }
}
