using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class EchoStateManager : MonoBehaviour
{

    [SerializeField] bool isPlayer = false;
    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer characterSprite;
    Animator anim;

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
    ToolUser toolUser;

    [Header("Hurt Params")]
    [SerializeField] float invulnerableDuration;
    [SerializeField] float hitFlashInterval;
    [SerializeField] float deathTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        characterSprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        toolUser = GetComponentInChildren<ToolUser>();
    }
    public void Activate()
    {
        acceptInputs = true;
        toolUser.Activate();
    }
    public void Deactivate()
    {
        toolUser.DeActivate();
        AcceptInputs(false);
        SetDashing(false);
        rb.linearVelocity = Vector2.zero;
        col.enabled = false;
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
            if (isDashing) rb.linearVelocity = dashDirection.normalized * dashSpeed;

            if (dashTimer > dashTimeSeconds + dashCooldownSeconds)
            {
                dashTimer = 0;
                SetDashReady(true);
            }
        }
    }

    public bool HandleInputs(InputData inputData)
    {
        if (!acceptInputs) return false;

        if (inputData.IsDashPress() && inputData.GetMoveDirection() != Vector2.zero) Dash(inputData.GetMoveDirection());
        if (isDashing) return true;

        toolUser.HandleInputs(inputData);
        SetShooting(inputData.IsMouseHold() && !isDashing);

        AimEcho(inputData.GetAimDirection()); // AimGun(inputData.GetAimDirection());
        Move(inputData.GetMoveDirection());
        return true;
    }

    void AimEcho(Vector2 direction)
    {
        characterSprite.flipX = direction.x < 0;
    }

    void Move(Vector2 direction)
    {
        bool movingValue = direction == Vector2.zero ? false : true;
        SetMoving(movingValue);
        rb.linearVelocity = direction.normalized * speed;
    }

    void Dash(Vector2 direction)
    {
        if (!dashReady) return;

        dashDirection = direction;
        SetDashing(true);
        SetDashReady(false);
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
