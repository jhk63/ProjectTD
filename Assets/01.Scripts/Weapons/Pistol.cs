using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    protected override void PerformFire()
    {
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, gunData.range))
        {
            // StartCoroutine(FireEffect(hit.point));

            if (hit.collider.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.TakeDamage(gunData.damage);
            }
        }
    }
}
