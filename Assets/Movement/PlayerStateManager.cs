using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{


    Rigidbody2D rb;
    SpriteRenderer playerSprite;


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
    SpriteRenderer gunSprite;
    [SerializeField] GameObject bullet;
    [SerializeField] float fireIntervalSeconds;
    [SerializeField] float gunDistanceFromBody;
    [SerializeField] LayerMask targetLayer;
    float fireTimer = 0f;
    bool fireReady = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gunSprite = gun.GetComponent<SpriteRenderer>();
        playerSprite = GetComponent<SpriteRenderer>();
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

        if (inputData.IsMouseHold() && fireReady) FireBullet(inputData.GetAimDirection());

        AimGun(inputData.GetAimDirection());
        Move(inputData.GetMoveDirection());
    }

    void AimGun(Vector2 direction)
    {
        // rotation
        float angleDegRaw = Vector2.Angle(direction, Vector2.right);
        float angleDeg = direction.y > 0 ? angleDegRaw : 180 + (180 - angleDegRaw);
        gun.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleDeg));

        // translation
        Vector3 positionFromPlayer = direction.normalized * gunDistanceFromBody;
        gun.transform.position = transform.position + positionFromPlayer;

        // flip player and gun graphics accordingly
        bool shouldFlip = direction.x < 0;
        gunSprite.flipY = shouldFlip;
        playerSprite.flipX = shouldFlip;

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
    void FireBullet(Vector2 direction)
    {
        SetFireReady(false);
        BulletFunctionality b = Instantiate(bullet, gun.transform.position, gun.transform.rotation).GetComponent<BulletFunctionality>();
        b.Initialize(direction.normalized, targetLayer);
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
