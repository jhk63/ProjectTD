using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : Gun
{
    protected override void PerformFire()
    {
        Vector3 direction = firePoint.forward;
        direction.x += Random.Range(-gunData.spread, gunData.spread);
        direction.y += Random.Range(-gunData.spread, gunData.spread);

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, direction, out hit, gunData.range))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.TakeDamage(gunData.damage);
            }
        }
    }
}
