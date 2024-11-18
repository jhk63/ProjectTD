using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] protected GunData gunData;
    [SerializeField] protected Transform firePoint;
    // [SerializeField] protected LayerMask targetLayer;

    // protected LineRenderer lineRenderer;

    protected float nextTimeFire = 0f;
    protected bool canFire;
    protected int currentAmmo;
    public int GetCurrentAmmo() { return currentAmmo; }

    private bool isFiring = false;
    private bool isReloading = false;
    public bool GetIsReloading() { return isReloading; }

    protected virtual void Awake()
    {
        currentAmmo = gunData.maxAmmo;
    }

    protected virtual void OnEnable()
    {
        canFire = true;
    }

    protected virtual void OnDisable()
    {
        canFire = false;
    }

    private void Update()
    {
        if (isFiring && gunData.isAutomatic)
        {
            Fire();
        }
    }

    public void StartFiring()
    {
        isFiring = true;
        Fire();
    }

    public void StopFiring()
    {
        isFiring = false;
        
    }

    public virtual void Fire()
    {
        if (!canFire || isReloading || Time.time < nextTimeFire) return;
        if (currentAmmo <= 0)
        {
            Debug.Log("Gun: Not Enough Ammo !");
            return;
        }

        nextTimeFire = Time.time + gunData.fireRate;
        currentAmmo--;

        PlayMuzzleEffect();
        PlayShootSound();
        // PerformFire();

    }

    protected abstract void PerformFire();

    public virtual void Reload()
    {
        if (isReloading) return;
        if (currentAmmo >= gunData.maxAmmo) return;

        StartCoroutine(PerformReload());
    }

    protected virtual IEnumerator PerformReload()
    {
        isReloading = true;

        if (gunData.reloadCilp != null)
        {
            AudioSource.PlayClipAtPoint(gunData.reloadCilp, transform.position);
        }

        // Debug.Log("Weapon: Reloading !");
        yield return new WaitForSeconds(gunData.reloadTime);

        if (gunData.reloadAfterCilp != null)
        {
            AudioSource.PlayClipAtPoint(gunData.reloadAfterCilp, transform.position);
        }

        currentAmmo = gunData.maxAmmo;
        isReloading = false;
        
        // Debug.Log("Gun: Reload Complete !");
    }

    protected virtual void PlayMuzzleEffect()
    {
        if (gunData.gunType == GunType.Shotgun) return;

        if (gunData.muzzleFlash != null)
        {
            Instantiate(gunData.muzzleFlash, firePoint.position, firePoint.rotation);
        }
    }

    protected virtual void PlayShootSound()
    {
        if (gunData.shootCilp != null)
        {
            AudioSource.PlayClipAtPoint(gunData.shootCilp, transform.position);
        }
    }
}
