using UnityEngine;

public class PiercerProjectileVisual : MonoBehaviour
{
    Rigidbody2D rb;
    float speed;
    Vector2 direction;
    float deathTimer = 5f;
    [SerializeField] GameObject particle;
    LayerMask optionalCollisionIgnores = 0;
    public void Initialize(Vector2 direction, float angle, float speed)
    {
        rb = GetComponent<Rigidbody2D>();
        this.rb.rotation = angle;
        this.direction = direction;
        this.speed = speed;
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
    void FizzleOut()
    {
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
