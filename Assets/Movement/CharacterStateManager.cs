using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateManager : MonoBehaviour
{

    [SerializeField] bool isPlayer = false;
    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer characterSprite;
    Animator anim;
    Health health;


    [Header("State Booleans")]
    [SerializeField] bool acceptInputs = false;
    [SerializeField] bool invulnerable;
    [SerializeField] bool isMoving;
    [SerializeField] bool isDashing;
    [SerializeField] bool isShooting;
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

    [Header("Hurt Params")]
    [SerializeField] float invulnerableDuration;
    [SerializeField] float hitFlashInterval;
    [SerializeField] float deathTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        gunSprite = gun.GetComponent<SpriteRenderer>();
        characterSprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        health.SetHitEvent(HitEvent);
        health.SetDeathEvent(DeathEvent);
        bulletDamageData = new(this.gameObject, bulletDamage);
    }
    public void ActivateCharacter()
    {
        acceptInputs = true;
        gun.SetActive(true);
    }

    void Update()
    {
        UpdateAnimatorParams();
        if (isDashing || !dashReady)
        {
            dashTimer += Time.deltaTime;

            if (dashTimer > dashTimeSeconds)
            {
                SetDashing(false);
            }
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

    public bool HandleInputs(InputData inputData)
    {
        if (!acceptInputs) return false;

        if (inputData.IsDashPress() && inputData.GetMoveDirection() != Vector2.zero) Dash(inputData.GetMoveDirection());
        if (isDashing) return true;

        if (inputData.IsMouseHold() && fireReady) FireBullet(inputData.GetAimDirection());
        SetShooting(inputData.IsMouseHold() && !isDashing);

        AimGun(inputData.GetAimDirection());
        Move(inputData.GetMoveDirection());
        return true;
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
        print(bulletDamageData.GetDamage());
        b.Initialize(direction.normalized, gun.transform.rotation.eulerAngles.z, bulletSpeed, bulletDamageData);
    }

    void HitEvent()
    {
        if (isPlayer)
        {
            StartCoroutine(InvulnerableCoroutine());
        }
    }
    IEnumerator InvulnerableCoroutine()
    {
        invulnerable = true;
        health.damageFilterList.Add(InvulnerableFrameDamageFilter);
        StartCoroutine(BlinkingFlash());

        yield return new WaitForSeconds(invulnerableDuration);

        health.damageFilterList.Remove(InvulnerableFrameDamageFilter);
        invulnerable = false;
    }
    IEnumerator BlinkingFlash()
    {
        while (invulnerable == true)
        {
            characterSprite.material.SetFloat("_FlashAmount", 1);
            yield return new WaitForSeconds(hitFlashInterval);
            characterSprite.material.SetFloat("_FlashAmount", 0);
            yield return new WaitForSeconds(hitFlashInterval);
            yield return null;
        }
    }
    DamageData InvulnerableFrameDamageFilter(DamageData data)
    {
        data.SetDamage(0);
        return data;
    }

    void DeathEvent()
    {
        anim.SetTrigger("death");
        AcceptInputs(false);
        rb.velocity = Vector2.zero;
        col.enabled = false;
        GlobalEventHolder.OnDeath?.Invoke(gameObject);
        StartCoroutine(DeathCoroutine());
    }
    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(deathTimer);
        Destroy(gameObject);
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
    void AcceptInputs(bool val)
    {
        acceptInputs = val;
    }
    void UpdateAnimatorParams()
    {
        anim.SetBool("moving", isMoving);
        anim.SetBool("dashing", isDashing);
        anim.SetBool("shooting", isShooting);
    }
}
