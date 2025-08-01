using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateManager : MonoBehaviour
{

    Rigidbody2D rb;
    SpriteRenderer characterSprite;


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
    DamageData bulletDamageData;
    [SerializeField] float bulletSpeed;
    [SerializeField] int bulletDamage;
    [SerializeField] float fireIntervalSeconds;
    [SerializeField] float gunDistanceFromBody;
    float fireTimer = 0f;
    bool fireReady = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gunSprite = gun.GetComponent<SpriteRenderer>();
        characterSprite = GetComponent<SpriteRenderer>();
        bulletDamageData = new(this.gameObject, bulletDamage);
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
        SetShooting(inputData.IsMouseHold() && !isDashing);

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

        // flip character and gun graphics accordingly
        bool shouldFlip = direction.x < 0;
        gunSprite.flipY = shouldFlip;
        characterSprite.flipX = shouldFlip;

    }
    void Move(Vector2 direction)
    {
        bool movingValue = direction == Vector2.zero ? false : true;
        SetMoving(movingValue);
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

        BulletFunctionality b = Instantiate(bullet, gun.transform.position, Quaternion.identity).GetComponent<BulletFunctionality>();
        b.Initialize(direction.normalized, gun.transform.rotation.eulerAngles.z, bulletSpeed, bulletDamageData);
    }

    void SetMoving(bool val)
    {
        isMoving = val;
    }
    void SetDashing(bool val)
    {
        isDashing = val;
    }
    void SetShooting(bool val)
    {
        isShooting = val;
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
