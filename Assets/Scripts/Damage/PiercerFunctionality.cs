using UnityEngine;

using VisualKits;


public class PiercerFunctionality : MonoBehaviour
{
    const int MAX_LENGTH = 100;

    Vector2 direction;
    DamageData damageData;
    float colliderRadius;


    bool firedOff = false;
    VisualKitManager visKit;
    GameObject chargeVisualObj;
    GameObject visualProjectile;
    LayerMask optionalCollisionIgnores = 0;

    public void Initialize(Vector2 direction, DamageData damage, float radius)
    {
        this.direction = direction;
        this.damageData = damage;
        this.colliderRadius = radius;

        visKit = GetComponentInChildren<VisualKitManager>();
        if (visKit)
        {
            VisualKit v = visKit.SelectKit(damage.GetAttacker().tag);
        }
    }
    public void OptionalCollisionIgnores(LayerMask layers)
    {
        this.optionalCollisionIgnores = layers;
    }

    public void SetModeTelegraph()
    {
        if (visKit)
        {
            chargeVisualObj.SetActive(true);
        }
    }
    public void SetModeFire(Vector2 direction)
    {
        gameObject.transform.parent = null;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, colliderRadius, direction, MAX_LENGTH);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.tag == this.gameObject.tag)
            {
                return;
            }
            TryHitTarget(hit.collider.gameObject);
        }
        if (visKit)
        {
            chargeVisualObj.SetActive(false);
            Instantiate(visualProjectile, transform.position, Quaternion.LookRotation(Vector3.forward, direction));
        }
        firedOff = true;
    }
    void Update()
    {
        if (firedOff)
        {
            // laserTrailLingerTime -= Time.deltaTime;
            // if (laserTrailLingerTime <= 0f)
            // {
            //     SetModeOff();
            // }
            SetModeOff();
        }
    }
    public void SetModeOff()
    {
        if (visKit)
        {
            chargeVisualObj.SetActive(false);
        }
        Destroy(this.gameObject);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == this.gameObject.tag)
        {
            return;
        }
        TryHitTarget(collision.gameObject);
    }
    bool TryHitTarget(GameObject target)
    {
        if (target == damageData.GetAttacker())
            return false;
        if (target.layer == damageData.GetAttackerLayer())
            return false;
        if (optionalCollisionIgnores != 0 &&
            (((1 << target.layer) & optionalCollisionIgnores.value) != 0)) // is there a layermask override - if so does it apply to the current target?
            return false;

        Health hp = target.GetComponent<Health>();
        if (hp != null)
            hp.Damage(new(damageData));

        return true;
    }
}
