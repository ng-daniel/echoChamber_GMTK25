using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReciever : MonoBehaviour
{


    Rigidbody2D rb;

    List<InputData> inputs = new List<InputData>();

    [Header("State Booleans")]
    [SerializeField] bool isMoving;
    [SerializeField] bool isDashing;
    [SerializeField] bool isShooting;
    [SerializeField] bool isHit;

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTimeSeconds;
    [SerializeField] float dashCooldownSeconds;
    float dashTimer = 0f;
    [SerializeField] bool dashReady = true;

    Vector2 dashDirection;
    [Header("Shooting")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject bullet;
    [SerializeField] float fireIntervalSeconds;
    [SerializeField] float gunDistanceFromBody;
    float fireTimer = 0f;
    bool fireReady = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDashing || !dashReady)
        {
            dashTimer += Time.deltaTime;

            if (dashTimer > dashTimeSeconds) SetDashing(false);
            if (isDashing) rb.velocity = dashDirection.normalized * dashSpeed;

            if (dashTimer > dashTimeSeconds + dashCooldownSeconds)
            {
                dashTimer = 0;
                SetDashReady(true);
            }
        }

        if (!fireReady)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer > fireIntervalSeconds)
            {
                fireTimer = 0;
                SetFireReady(true);
            }
        }
    }

    public void HandleInputs(InputData inputData)
    {
        //print(inputData);
        if (inputData.IsDashPress() && inputData.GetMoveDirection() != Vector2.zero)
        {
            Dash(inputData.GetMoveDirection());
        }

        if (isDashing) return;

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
        if (direction != Vector2.zero)
        {
            SetMoving(false);
        }
        else
        {
            SetMoving(true);
        }
        rb.velocity = direction.normalized * speed;
    }

    void Dash(Vector2 direction)
    {
        if (!dashReady) return;

        dashDirection = direction;
        SetDashing(true);
        SetDashReady(false);
    }
    void FireBullet()
    {
        SetFireReady(false);
    }

    void SetMoving(bool val)
    {
        isMoving = val;
    }
    void SetDashing(bool val)
    {
        isDashing = val;
    }
    void SetFireReady(bool val)
    {
        fireReady = val;
    }
    void SetDashReady(bool val)
    {
        dashReady = val;
    }
}
