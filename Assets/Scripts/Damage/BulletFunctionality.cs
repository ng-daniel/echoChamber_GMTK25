using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using Tools;
using VisualKits;

public class BulletFunctionality : MonoBehaviour, IDamaging
{
    Health hp;
    Rigidbody2D rb;
    float speed;
    DamageData damageData;
    [SerializeField] string damageDataContext;
    Vector2 direction;
    float deathTimer = 5f;
    [SerializeField] GameObject particle;
    VisualKitManager visKit;
    LayerMask optionalCollisionIgnores = 0;

    public void Initialize(Vector2 direction, float angle, float speed, DamageData damage, ToolUserConfig config = null)
    {
        rb = GetComponent<Rigidbody2D>();
        this.rb.rotation = angle;
        this.direction = direction;
        this.speed = speed;

        this.damageData = damage;
        damageData.SetContext(damageDataContext);

        visKit = GetComponentInChildren<VisualKitManager>();
        if (visKit)
        {
            visKit.SelectKit(damage.GetAttacker().tag);
        }

        hp = this.gameObject.GetComponent<Health>();
        hp.SetDeathEvent(FizzleOut);
    }

    public void OptionalCollisionIgnores(LayerMask layers)
    {
        this.optionalCollisionIgnores = layers;
    }

    void Update()
    {
        rb.linearVelocity = direction.normalized * speed;

        rb.rotation = rb.rotation >= 360 ? 0 : rb.rotation;
        rb.rotation = rb.rotation < 0 ? 360 : rb.rotation;

        deathTimer -= Time.deltaTime;
        if (deathTimer < 0)
        {
            FizzleOut();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        bool hitResult = TryHitTarget(collision.gameObject);
        if (hitResult) FizzleOut();
    }
    bool TryHitTarget(GameObject target)
    {
        return IDamaging.TryHitTarget(target, damageData, optionalCollisionIgnores);
    }
    void FizzleOut()
    {
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    public DamageData GetDamageData()
    {
        return damageData;
    }
}
