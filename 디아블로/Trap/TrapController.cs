using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    public float damageInterval = 0.5f;
    public float damageDuration = 5f;
    public int damage = 5;

    private float calcDuration = 0.0f;

    [SerializeField] private ParticleSystem effect;
    private IDamagable damagable;

    private void Update()
    {
        if (damagable != null)
        {
            calcDuration -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        damagable = other.GetComponent<IDamagable>();
        if (damagable != null)
        {
            calcDuration = damageDuration;
            effect.Play();
            StartCoroutine(ProcessDamage());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
        damagable = null;
        effect.Stop();
    }

    private IEnumerator ProcessDamage()
    {
        while (calcDuration > 0 && damagable != null)
        {
            damagable.TakeDamage(damage, null);
            yield return new WaitForSeconds(damageInterval);
        }

        damagable = null;
        effect.Stop();
    }
}
