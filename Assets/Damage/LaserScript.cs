using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserScript : MonoBehaviour
{

    [SerializeField] LineRenderer bigLineRenderer;
    [SerializeField] LineRenderer littleLineRenderer;
    [SerializeField] float minLittleWidth;
    [SerializeField] float maxLittleWidth;
    const int LASER_OFF = 0;
    const int BIG_LASER = 1;
    const int LITTLE_LASER = 2;
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


    public void Initialize(GameObject attacker, int damage)
    {
        this.damageData = new(attacker, damage);
        damageData.SetContext(context);
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        int damage = 1;
        Initialize(this.gameObject, damage);
        RotateAttackChain(Vector2.left + Vector2.down);
    }

    public void RotateAttackChain(Vector2 initialDirection)
    {
        if (rotateAttackRunning) return;
        rotateChargeTimer = 0;
        rotateFireTimer = 0;

        SetBodyRotation(Vector2.Angle(Vector2.right, initialDirection));
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
        while (rotateWindDownTimer < rotateFireTimeSec)
        {
            AddBodyRotation(rotateRate * Time.deltaTime);
            rotateRate -= rotateRate > 0 ? windDownRotateDecrementSec * Time.deltaTime : 0;

            rotateWindDownTimer += Time.deltaTime;
            yield return null;
        }

        ToggleLaserRenderer(LASER_OFF);
        rotateAttackRunning = false;
    }

    void FireLaserFunction(Vector2 aimDirection)
    {
        // raycast to gague distance
        RaycastHit2D hitRay = Physics2D.Raycast(this.transform.position, aimDirection, maxDistance, stopLayers);
        Vector2 hitPosition = hitRay ? hitRay.point : aimDirection * maxDistance;

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
        }
        if (code == BIG_LASER)
        {
            littleLineRenderer.gameObject.SetActive(false);
            bigLineRenderer.gameObject.SetActive(true);
        }
        if (code == LITTLE_LASER)
        {
            littleLineRenderer.gameObject.SetActive(true);
            bigLineRenderer.gameObject.SetActive(false);
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
            renderer.startWidth = lerp.Operation(startWidth, targetWidth, timeVal / duration);
            timeVal -= Time.deltaTime;
            yield return null;
        }
    }
}
