using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletFunctionality : MonoBehaviour
{

    Rigidbody2D rb;

    float speed;
    [SerializeField] float rotationSpeed;
    int damage;
    DamageData damageData;
    [SerializeField] string damageDataContext;
    Vector2 direction;
    float deathTimer = 15f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, float speed, DamageData damage)
    {
        this.direction = direction;
        this.damageData = damage;
        damageData.SetContext(damageDataContext);
        this.speed = speed;
    }

    void Update()
    {
        rb.velocity = direction.normalized * speed;

        rb.rotation += rotationSpeed * Time.deltaTime;
        rb.rotation = rb.rotation >= 360 ? 0 : rb.rotation;
        rb.rotation = rb.rotation < 0 ? 360 : rb.rotation;

        deathTimer -= Time.deltaTime;
        if (deathTimer < 0)
        {
            //FizzleOut();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        print("hit something!");
        bool hitResult = TryHitTarget(collision.gameObject);
        if (hitResult) FizzleOut();
    }
    bool TryHitTarget(GameObject target)
    {
        if (target == damageData.GetAttacker()) return false;
        Health hp = target.GetComponent<Health>();
        if (hp == null) return true;
        hp.Damage(damageData);
        return true;
    }
    void FizzleOut()
    {
        Destroy(this.gameObject);
    }
}
