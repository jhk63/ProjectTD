using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    Pistol,
    Rifle,
    Shotgun
}

[CreateAssetMenu(fileName = "New GunData", menuName = "Scriptable Object/GunData")]
public class GunData : ScriptableObject
{
    // Info
    [Header("Info")]
    public string gunName;
    public GunType gunType;
    public GameObject gunPrefab;
    public Texture2D gunSprite;

    // Stats
    [Header("Stats")]
    public int maxAmmo;
    public float damage;
    public float fireRate;
    public float range;
    public float reloadTime;

    public bool isAutomatic;

    // Rifle
    [Header("Rifle")]
    public float spread;

    // Shotgun
    [Header("Shotgun")]
    // public GameObject pelletPrefab;
    public int pelletCount;
    public float spreadAngle;

    // Effects
    [Header("Effects")]
    public GameObject muzzleFlash;
    // public GameObject hitEffect;
    public AudioClip shootCilp;
    public AudioClip reloadCilp;
    public AudioClip reloadAfterCilp;
}
