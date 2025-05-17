using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Movement : MonoBehaviourPunCallbacks
{
    private HaritaYapýcý HaritaYapýcý;
    [Header("Bileþenler")]
    public Camera playerCamera;
    public FixedJoystick FixedJoystick;
    public GameObject JumpButton;
    public GameObject PauseButton;
    public GameObject EnvanterButton;
    public GameObject SpeedButton;
    public TMP_Text OyunucuAdý;
    public GameObject EnvanterPaneli;

    [Header("Hareket Ayarlarý")]
    public int hýz = 5;
    public int zýplama = 5;

    [Header("FOV Ayarlarý")]
    public float normalFov = 60f;
    public float kosmaFov = 75f;
    public float fovDegismeHýzý = 5f;

    [Header("Can Ayarlarý")]
    public Slider healthBar; // Can barý referansý
    public float maxHealth = 100f; // Maksimum can
    private float currentHealth; // Þu anki can

    [Header("Stamina Ayarlarý")]
    public Slider staminaBar; // Stamina barý referansý
    public float maxStamina = 100f; // Maksimum stamina
    private float currentStamina; // Þu anki stamina
    public float staminaRegenRate = 5f; // Staminanýn yenilenme hýzý
    public float staminaCost = 10f; // Koþma sýrasýnda harcanan stamina
    private bool isMoving = false; // Hareket edip etmediðini kontrol et

    private Rigidbody rb;
    private bool isGround = true;
    private bool BasýldýMý = false;
    private int Koþma = 1; // 1 = normal, 2 = koþma

    [Header("Koþma Butonu")]
    public Image SpeedButtonImage; // Butonun image bileþeni
    public Sprite yurumeSprite; // Yürüme sprite'ý
    public Sprite kosmaSprite; // Koþma sprite'ý

    void Start()
    {
        HaritaYapýcý = FindObjectOfType<HaritaYapýcý>();
        OyunucuAdý.text = photonView.Owner.NickName;

        rb = GetComponent<Rigidbody>();

        // Can ve stamina bar ayarlarý
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = currentStamina;

        if (photonView.IsMine!)
        {
            ShowLocalUI(false);
        }
    }

    void ShowLocalUI(bool aktif)
    {
        if (playerCamera) playerCamera.gameObject.SetActive(aktif);
        if (FixedJoystick) FixedJoystick.gameObject.SetActive(aktif);
        if (JumpButton) JumpButton.SetActive(aktif);
        if (PauseButton) PauseButton.SetActive(aktif);
        if (EnvanterButton) EnvanterButton.SetActive(aktif);
        if (SpeedButton) SpeedButton.SetActive(aktif);
        if (OyunucuAdý) OyunucuAdý.gameObject.SetActive(!aktif);
        if (staminaBar) staminaBar.gameObject.SetActive(aktif);
        if (healthBar) healthBar.gameObject.SetActive(aktif);
        if (EnvanterPaneli) EnvanterPaneli.gameObject.SetActive(aktif);
    }
    void Update()
    {
        if (!photonView.IsMine)
        {
            return; // Sadece baþkasýnýn karakteri için çýk
        }

        if (HaritaYapýcý.BaþladýMý)
        {
            ShowLocalUI(true);
        }
        else if (!HaritaYapýcý.BaþladýMý)
        {
            ShowLocalUI(false);
        }
        // Aktif hýzý belirle
        float aktifHýz = hýz * Koþma;

        // Joystick girdileri
        float horizontal = FixedJoystick.Horizontal * aktifHýz;
        float vertical = FixedJoystick.Vertical * aktifHýz;

        // Kameranýn yönüne göre hareket
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = playerCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 hareket = cameraForward * vertical + cameraRight * horizontal;

        // Hareket uygula
        rb.velocity = new Vector3(hareket.x, rb.velocity.y, hareket.z);

        // FOV deðiþimi
        float hedefFov = Koþma == 2 ? kosmaFov : normalFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, hedefFov, Time.deltaTime * fovDegismeHýzý);

        // Stamina yenileme
        if (currentStamina < maxStamina)
        {
            if (Koþma == 1)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
                staminaBar.value = currentStamina;
            }
        }

        // Koþma sýrasýnda stamina kaybý sadece hareket edildiðinde
        if (Koþma == 2 && isMoving && currentStamina > 0)
        {
            currentStamina -= staminaCost * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            staminaBar.value = currentStamina;

            // Eðer stamina 0'a düþtüyse, yürüme moduna geç
            if (currentStamina <= 0)
            {
                Koþma = 1;
                SpeedButtonImage.sprite = yurumeSprite; // Koþma yerine yürüme sprite'ýný göster
            }
        }

        // Eðer hareket etmiyorsanýz, isMoving false olsun
        if (horizontal == 0 && vertical == 0)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }
    void LateUpdate()
    {
        if (!photonView.IsMine && OyunucuAdý != null && Camera.main != null)
        {
            Vector3 direction = Camera.main.transform.position - OyunucuAdý.transform.position;
            direction.y = 0f; // Sadece Y ekseninde dönsün, yukarý-aþaðý bakmasýn
            if (direction.sqrMagnitude > 0.001f) // Çok yakýnda deðilse
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                OyunucuAdý.transform.rotation = lookRotation;
            }
        }
    }

    public void Jump()
    {
        if (!photonView.IsMine) return;

        if (isGround)
        {
            rb.velocity = new Vector3(rb.velocity.x, zýplama, rb.velocity.z);
            isGround = false;
        }
    }

    public void speedButon()
    {
        BasýldýMý = !BasýldýMý;
        Koþma = BasýldýMý ? 2 : 1;

        // Koþma butonunun sprite'ýný deðiþtir
        if (Koþma == 2)
        {
            SpeedButtonImage.sprite = kosmaSprite; // Koþma modunda koþma sprite'ý göster
        }
        else
        {
            SpeedButtonImage.sprite = yurumeSprite; // Yürüme modunda yürüme sprite'ý göster
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }
}
