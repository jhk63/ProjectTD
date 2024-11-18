using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    // References
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private GunData[] availableGuns;
    private Gun[] weaponInventory;
    private Gun currentWeapon;
    private int currentWeaponIndex = 0;

    // Settings
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;

    private float gravity = -30f;
    private Vector3 velocity;
    private bool isGround;

    [SerializeField] private float maxHP = 100f;
    private float currentHP;

    [SerializeField] private GameObject HpBar;
    private PlayerHpBar playerHpBar;
    [SerializeField] private GameObject Ammo;
    private PlayerAmmo playerAmmo;
    [SerializeField] private GameObject GunImage;
    private PlayerGunImage playerGunImage;

    public  UnityEvent OnDie;

    // Roll -> Dash
    private float rollSpeed = 10f;
    private float rollDuration = 0.5f;
    private float rollCooldown = 1f;

    private Vector3 rollDirection;

    private CharacterController characterController;
    private Animator animator;
    private PlayerInputActions playerInputActions;
    
    private Vector2 moveInput;
    private Vector2 lookInput;

    private bool isAlive = true;
    private bool isAiming = false;
    private bool canFire = true;
    private bool canRoll = true;
    private bool isRolling = false;
    private bool isDialogue = false;

    public bool keyEPressed = false;

    private float mouseRClicked = 0;

    private void SetAiming(bool isAiming)
    {
        this.isAiming = isAiming;
        animator.SetBool("isAiming", this.isAiming);
    }

    public void SetDiagloue(bool isDialogue)
    {
        this.isDialogue = isDialogue;
    }

    public float GetGunDamage()
    {
        return availableGuns[currentWeaponIndex].damage;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        // animator = GetComponent<Animator>();
        animator = GetComponentInChildren<Animator>();
        playerInputActions = new PlayerInputActions();

        // Input System Binding
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMove;
        playerInputActions.Player.Look.performed += OnLook;
        playerInputActions.Player.Aiming.performed += OnAimingStarted;
        playerInputActions.Player.Aiming.canceled += OnAimingCanceled;
        playerInputActions.Player.Fire.performed += OnFire;
        playerInputActions.Player.Fire.canceled += OnFireCanceled;
        playerInputActions.Player.Roll.performed += OnRoll;
        playerInputActions.Player.Reload.performed += OnReload;
        playerInputActions.Player.WeaponSelect.performed += OnWeaponSelect;
        
        // Gundata
        if (availableGuns.Length == 0) return;

        weaponInventory = new Gun[availableGuns.Length];
        for (int i=0; i < availableGuns.Length; i++)
        {
            GameObject weapon = Instantiate(availableGuns[i].gunPrefab, weaponTransform);
            weapon.transform.SetParent(weaponTransform);
            weaponInventory[i] = weapon.GetComponent<Gun>();
            weapon.SetActive(false);
        }
        SwitchWeapon(0);

        // StatsUI
        currentHP = maxHP;

        playerHpBar = HpBar.GetComponent<PlayerHpBar>();
        playerHpBar.SetValue(currentHP);

        playerAmmo = Ammo.GetComponent<PlayerAmmo>();
        playerAmmo.SetText(availableGuns[currentWeaponIndex].maxAmmo, currentWeapon.GetCurrentAmmo());

        playerGunImage = GunImage.GetComponent<PlayerGunImage>();
        playerGunImage.SetImage(availableGuns[currentWeaponIndex].gunSprite);
    }

    private void OnEnable()
    {
        isAlive = true;

        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    private void Update()
    {
        if (!isAlive) return;
        if (isDialogue) return;

        isGround = characterController.isGrounded;
        if (isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (!isRolling)
        {
            Movement();
            Rotation();
        }
        else 
        {
            Roll();
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        playerAmmo.SetText(availableGuns[currentWeaponIndex].maxAmmo, currentWeapon.GetCurrentAmmo());
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Acid"))
        {
            TakeDamage(10f);
        }
    }

    private void Movement()
    {
        if (!isAlive) return;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        characterController.Move(move * (moveSpeed * Time.deltaTime));
        

        if (move != Vector3.zero)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void Rotation()
    {
        if (!isAlive) return;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        if (isAiming)
        {
            Ray ray = Camera.main.ScreenPointToRay(lookInput);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                Vector3 lookPoint = ray.GetPoint(rayDistance);
                lookPoint.y = transform.position.y;
                Vector3 lookDirection = lookPoint - transform.position;
                
                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    modelTransform.rotation = Quaternion.Lerp(modelTransform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
                }
            }
        }
        else if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            modelTransform.rotation = Quaternion.Lerp(modelTransform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
    }

    private void Roll()
    {
        if (!isAlive) return;

        characterController.Move(rollDirection * (rollSpeed * Time.deltaTime));
    }

    private IEnumerator PerformRoll()
    {
        isRolling = true;
        canRoll = false;

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        rollDirection = moveDirection.magnitude > 0.1f ? 
            moveDirection.normalized : modelTransform.forward;

        // animation

        float rollTimer = 0f;
        while (rollTimer < rollDuration)
        {
            rollTimer += Time.deltaTime;
            yield return null;
        }

        isRolling = false;

        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;

    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;
        if (isRolling) return;

        // Debug.Log("Player: TakeDamage");
        currentHP -= damage;
        playerHpBar.SetValue(currentHP);

        if (currentHP <= 0)
        {
            currentHP = 0;
            isAlive = false;
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        playerInputActions.Disable();

        StartCoroutine(EndDieAnim());
    }

    private IEnumerator EndDieAnim()
    {
        yield return new WaitForSeconds(2f);

        OnDie.Invoke();
    }

    private void SwitchWeapon(int index)
    {
        if (isAiming) return;
        if (index < 0 || index >= weaponInventory.Length) return;

        if (currentWeapon != null)
        {
            if (currentWeapon.GetIsReloading()) return;

            currentWeapon.gameObject.SetActive(false);
        }

        currentWeaponIndex = index;
        currentWeapon = weaponInventory[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);

        if (playerGunImage != null)
        {
            playerGunImage.SetImage(availableGuns[currentWeaponIndex].gunSprite);
        }
    }


    #region Input System Binding
    // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/PlayerInput.html
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnAimingStarted(InputAction.CallbackContext context)
    {
        SetAiming(true);
        mouseRClicked = context.ReadValue<float>();
    }

    private void OnAimingCanceled(InputAction.CallbackContext context)
    {
        SetAiming(false);
        mouseRClicked = context.ReadValue<float>();
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (!isAlive) return;
        if (!canFire) return;
        if (isDialogue) return;

        if (currentWeapon == null) return;

        // Debug.Log("Player: OnFire !");
        animator.SetBool("isAiming", true);
        // currentWeapon.Fire();
        currentWeapon.StartFiring();
        SetAiming(true);
    }

    private void OnFireCanceled(InputAction.CallbackContext context)
    {
        if (mouseRClicked == 0)
        {
            SetAiming(false);
        }
        
        if (currentWeapon == null) return;

        currentWeapon.StopFiring();
    }

    private void OnRoll(InputAction.CallbackContext context)
    {
        if (!canRoll || isRolling) return;

        StartCoroutine(PerformRoll());
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        if (!isAlive) return;

        if (currentWeapon == null) return;

        Debug.Log("Player: Reload !");
        currentWeapon.Reload();
    }

    private void OnWeaponSelect(InputAction.CallbackContext context)
    {
        int weaponIndex = (int)context.ReadValue<float>();

        SwitchWeapon(weaponIndex);
        
    }

    #endregion

}