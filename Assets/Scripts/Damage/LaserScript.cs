using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserScript : MonoBehaviour
{

    [SerializeField] LineRenderer bigLineRenderer;
    [SerializeField] LineRenderer littleLineRenderer;
    [SerializeField] GameObject beamParticles;
    [SerializeField] GameObject splashParticles;
    [SerializeField] float minLittleWidth;
    [SerializeField] float maxLittleWidth;
    const int LASER_OFF = 0;
    const int BIG_LASER = 1;
    const int LITTLE_LASER = 2;
    const int defaultDamage = 1;
    Rigidbody2D rb;

    [Header("Laser Config Settings")]
    [SerializeField] float maxDistance; // to completely ensure no infinite cast BS
    [SerializeField] float laserWidth;
    [SerializeField] LayerMask stopLayers;
    [SerializeField] LayerMask damageLayers;
    [SerializeField] DamageData damageData;
    [SerializeField] string context;


    [Header("Effect Timer Params")]
    [SerializeField] float flickerInterval;
    [SerializeField] float windDownRotateDecrementSec;

    [Header("Rotate Attack Params")]
    bool rotateAttackRunning = false;
    [SerializeField] float rotateDegrees; // per second
    [SerializeField] float rotateChargeTimeSec;
    [SerializeField] float rotateFireTimeSec;
    [SerializeField] float rotateWindDownTimeSec;
    float rotateChargeTimer;
    float rotateFireTimer;
    float rotateWindDownTimer;


    public void Initialize(GameObject attacker, LaserParameters p)
    {
        rb = GetComponent<Rigidbody2D>();

        if (p != null)
        {
            this.damageData = new(attacker, p.GetDamage());
            this.rotateDegrees = p.GetRotateDegrees();
            this.rotateChargeTimeSec = p.GetRotateChargeTimeSec();
            this.rotateFireTimeSec = p.GetRotateFireTimeSec();
            this.rotateWindDownTimeSec = p.GetRotateWindDownTimeSec();
            this.windDownRotateDecrementSec = p.GetWindDownRotateDecrementSec();
        }
        else
        {
            this.damageData = new(attacker, defaultDamage);
        }
        damageData.SetContext(context);
    }
    public void RotateAttackChain(Vector2 initialDirection)
    {
        if (rotateAttackRunning) return;
        rotateChargeTimer = 0;
        rotateFireTimer = 0;


        float angleDegRaw = Vector2.Angle(initialDirection, Vector2.right);
        float angleDeg = initialDirection.y > 0 ? angleDegRaw : 180 + (180 - angleDegRaw);
        SetBodyRotation(angleDeg);
        ToggleLaserRenderer(LITTLE_LASER);
        StartCoroutine(RotateChargeCoroutine());
    }

    IEnumerator RotateChargeCoroutine()
    {
        StartCoroutine(FlickerLaser(littleLineRenderer, flickerInterval, rotateChargeTimeSec));
        StartCoroutine(AdjustLaserWidth(littleLineRenderer, rotateChargeTimeSec, minLittleWidth, maxLittleWidth, new ScalarLerp()));
        rotateAttackRunning = true;
        while (rotateChargeTimer < rotateChargeTimeSec)
        {
            AddBodyRotation(rotateDegrees * Time.deltaTime);
            SetLineRendererEndPoints(
                transform.position,
                GetLaserDistance(GetDirectionFromRotation(rb.rotation))
            );

            rotateChargeTimer += Time.deltaTime;
            yield return null;
        }

        ToggleLaserRenderer(BIG_LASER);
        StartCoroutine(RotateFireCoroutine());
    }
    IEnumerator RotateFireCoroutine()
    {
        while (rotateFireTimer < rotateFireTimeSec)
        {
            FireLaserFunction(GetDirectionFromRotation(rb.rotation));
            AddBodyRotation(rotateDegrees * Time.deltaTime);

            rotateFireTimer += Time.deltaTime;
            yield return null;
        }

        ToggleLaserRenderer(LITTLE_LASER);
        StartCoroutine(RotateWindDownCoroutine(rotateDegrees));
    }
    IEnumerator RotateWindDownCoroutine(float rotateRate)
    {
        StartCoroutine(FlickerLaser(littleLineRenderer, flickerInterval, rotateWindDownTimeSec));
        StartCoroutine(AdjustLaserWidth(littleLineRenderer, rotateWindDownTimeSec, maxLittleWidth, minLittleWidth, new ScalarLerp()));
        while (rotateWindDownTimer < rotateWindDownTimeSec)
        {
            AddBodyRotation(rotateRate * Time.deltaTime);
            SetLineRendererEndPoints(
                transform.position,
                GetLaserDistance(GetDirectionFromRotation(rb.rotation))
            );
            rotateRate -= rotateRate > 0 ? windDownRotateDecrementSec * Time.deltaTime : 0;

            rotateWindDownTimer += Time.deltaTime;
            yield return null;
        }

        ToggleLaserRenderer(LASER_OFF);
        rotateAttackRunning = false;

        Destroy(gameObject);
    }

    Vector2 GetLaserDistance(Vector2 aimDirection)
    {
        RaycastHit2D hitRay = Physics2D.Raycast(this.transform.position, aimDirection, maxDistance, stopLayers);
        return hitRay ? hitRay.point : aimDirection * maxDistance;
    }

    void FireLaserFunction(Vector2 aimDirection)
    {
        // raycast to gague distance
        Vector2 hitPosition = GetLaserDistance(aimDirection);

        // circlecast to collide and damage
        RaycastHit2D[] hitCircle = Physics2D.CircleCastAll(this.transform.position, laserWidth / 2, aimDirection, maxDistance, damageLayers);
        foreach (RaycastHit2D hit in hitCircle)
        {
            Health objectHealth = hit.collider.gameObject.GetComponent<Health>();
            if (objectHealth == null) continue;

            print("object: " + objectHealth.gameObject);
            objectHealth.Damage(damageData);
        }
        SetLineRendererEndPoints(this.transform.position, hitPosition);
        splashParticles.transform.position = hitPosition;
    }
    void SetBodyRotation(float val)
    {
        rb.rotation = val;
    }
    void AddBodyRotation(float val)
    {
        rb.rotation += val;
    }
    Vector2 GetDirectionFromRotation(float val)
    {
        float angle = Mathf.Deg2Rad * rb.rotation;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    void ToggleLaserRenderer(int code)
    {
        if (code == LASER_OFF)
        {
            littleLineRenderer.gameObject.SetActive(false);
            bigLineRenderer.gameObject.SetActive(false);
            splashParticles.SetActive(false);
            beamParticles.SetActive(false);
        }
        if (code == BIG_LASER)
        {
            littleLineRenderer.gameObject.SetActive(false);
            bigLineRenderer.gameObject.SetActive(true);
            splashParticles.SetActive(true);
            beamParticles.SetActive(true);
        }
        if (code == LITTLE_LASER)
        {
            littleLineRenderer.gameObject.SetActive(true);
            bigLineRenderer.gameObject.SetActive(false);
            splashParticles.SetActive(false);
            beamParticles.SetActive(true);
        }
    }
    void SetLineRendererEndPoints(Vector2 start, Vector2 end)
    {
        littleLineRenderer.SetPosition(0, start);
        bigLineRenderer.SetPosition(0, start);

        littleLineRenderer.SetPosition(1, end);
        bigLineRenderer.SetPosition(1, end);
    }
    IEnumerator FlickerLaser(LineRenderer renderer, float interval, float duration)
    {
        while (duration > 0 && renderer.gameObject.activeSelf)
        {
            duration -= interval;
            renderer.enabled = !renderer.enabled;
            yield return new WaitForSeconds(interval);
        }
    }
    IEnumerator AdjustLaserWidth(LineRenderer renderer, float duration, float startWidth, float targetWidth, ScalarLerp lerp, float timeVal = 0)
    {
        while (timeVal > 0 && renderer.gameObject.activeSelf)
        {
            renderer.widthMultiplier = lerp.Operation(startWidth, targetWidth, timeVal / duration);
            timeVal -= Time.deltaTime;
            yield return null;
        }
    }
}
