using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Variables

    public float speed;
    public GameObject muzzleFlashPrefabs;
    public GameObject hitEffectPrefab;
    
    public AudioClip shotSFX;
    public AudioClip hitSFX;

    private bool isCollided = false;
    private Rigidbody rigid;

    [HideInInspector] public AttackBehaviour attackBehaviour;
    [HideInInspector] public GameObject owner;
    [HideInInspector] public GameObject target;

    #endregion Variables

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!(target is null))
        {
            Vector3 t_dest = target.transform.position;
            t_dest.y += 1.5f;
            transform.LookAt(t_dest);
        }

        if(owner) 
        {
            Collider t_projectileCollider = GetComponent<Collider>();
            Collider[] t_ownerColliders = owner.GetComponentsInChildren<Collider>();

            foreach (Collider t_collider in t_ownerColliders)
            {
                Physics.IgnoreCollision(t_projectileCollider, t_collider);
            }
        }

        if (muzzleFlashPrefabs)
        {
            GameObject t_muzzleVFX = Instantiate(muzzleFlashPrefabs, transform.position, Quaternion.identity);
            t_muzzleVFX.transform.forward = gameObject.transform.forward;
            ParticleSystem t_particle = t_muzzleVFX.GetComponent<ParticleSystem>();
            if (t_particle) Destroy(t_muzzleVFX, t_particle.main.duration);
            else
            {
                ParticleSystem t_childParticle = t_muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                if (t_childParticle) Destroy(t_muzzleVFX, t_childParticle.main.duration);
            }
        }

        if (!(shotSFX is null) && GetComponent<AudioSource>())
            GetComponent<AudioSource>().PlayOneShot(shotSFX);
    }

    protected virtual void FixedUpdate()
    {
        if (speed != 0 && !(rigid is null))
        {
            rigid.position += transform.position * (speed * Time.deltaTime);
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (isCollided) return;

        isCollided = true;
        Collider t_projectileColldier = GetComponent<Collider>();
        t_projectileColldier.enabled = false;

        if (!(hitSFX is null) && GetComponent<AudioSource>())
            GetComponent<AudioSource>().PlayOneShot(hitSFX);

        speed = 0;
        rigid.isKinematic = true;

        ContactPoint t_contact = collision.contacts[0];
        Quaternion t_contactRot = Quaternion.FromToRotation(Vector3.up, t_contact.normal);
        Vector3 t_contactPos = t_contact.point;

        if (!(hitEffectPrefab is null))
        {
            GameObject t_hitVFX = Instantiate(hitEffectPrefab, t_contactPos, t_contactRot);
            ParticleSystem t_particle = t_hitVFX.GetComponent<ParticleSystem>();
            if (t_particle) Destroy(t_hitVFX, t_particle.main.duration);
            else
            {
                ParticleSystem t_childParticle = t_hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                if (t_childParticle) Destroy(t_hitVFX, t_childParticle.main.duration);
            }
        }

        IDamagable t_damagable = collision.gameObject.GetComponent<IDamagable>();
        if (t_damagable != null)
        {
            t_damagable.TakeDamage(attackBehaviour?.damage ?? 0, null);
        }

        StartCoroutine(DestroyParticle(5.0f));
    }

    public IEnumerator DestroyParticle(float p_delay)
    {
        if (transform.childCount > 0 && p_delay != 0)
        {
            List<Transform> t_children = new List<Transform>();
            foreach (Transform t in t_children)
            {
                t_children.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < t_children.Count; ++i)
                    t_children[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            }
        }

        yield return new WaitForSeconds(p_delay);
        Destroy(gameObject);
    }
}
