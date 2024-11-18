using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    protected override void PerformFire()
    {
        for (int i=0; i < gunData.pelletCount; i++)
        {
            Quaternion randomRotation = Quaternion.Euler(0, 
                Random.Range(-gunData.spreadAngle, gunData.spreadAngle), 0);

            // GameObject bullet = Instantiate(gunData.pelletPrefab, firePoint.position, firePoint.rotation * randomRotation);
        }
    }

    protected override void PlayMuzzleEffect()
    {
        if (gunData.muzzleFlash == null) return;

        for (int i=0; i < gunData.pelletCount; i++)
        {
            GameObject bullet = Instantiate(gunData.muzzleFlash, firePoint.position, firePoint.rotation);

            float randomSpread = Random.Range(-gunData.spreadAngle, gunData.spreadAngle);
            bullet.transform.RotateAround(firePoint.position, Vector3.up, randomSpread);

            Destroy(bullet, 0.4f);
        }
    }
}
