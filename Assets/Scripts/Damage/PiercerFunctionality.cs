using UnityEngine;

using VisualKits;


public class PiercerFunctionality : MonoBehaviour
{
    const int MAX_LENGTH = 100;
    DamageData damageData;
    Collider2D piercerCollider;
    VisualKitManager visKit;
    [SerializeField] float laserTrailLingerTime;
    GameObject piercerVisualTelegraph;
    GameObject piercerVisualFire;
    [SerializeField] GameObject particle;
    [SerializeField] LayerMask optionalCollisionIgnores = 0;

    public void Initialize(DamageData damage)
    {
        this.damageData = damage;
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
        piercerCollider.enabled = false;
        if (visKit)
        {
            piercerVisualTelegraph.SetActive(true);
            piercerVisualFire.SetActive(false);
        }
    }
    public void SetModeFire()
    {
        gameObject.transform.parent = null;
        piercerCollider.enabled = true;
        if (visKit)
        {
            piercerVisualTelegraph.SetActive(false);
            piercerVisualFire.SetActive(true);
        }
    }
    public void SetModeOff()
    {
        piercerCollider.enabled = false;
        if (visKit)
        {
            piercerVisualTelegraph.SetActive(false);
            piercerVisualFire.SetActive(false);
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
