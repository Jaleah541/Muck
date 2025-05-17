using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Movement : MonoBehaviourPunCallbacks
{
    private HaritaYap�c� HaritaYap�c�;
    [Header("Bile�enler")]
    public Camera playerCamera;
    public FixedJoystick FixedJoystick;
    public GameObject JumpButton;
    public GameObject PauseButton;
    public GameObject EnvanterButton;
    public GameObject SpeedButton;
    public TMP_Text OyunucuAd�;
    public GameObject EnvanterPaneli;

    [Header("Hareket Ayarlar�")]
    public int h�z = 5;
    public int z�plama = 5;

    [Header("FOV Ayarlar�")]
    public float normalFov = 60f;
    public float kosmaFov = 75f;
    public float fovDegismeH�z� = 5f;

    [Header("Can Ayarlar�")]
    public Slider healthBar; // Can bar� referans�
    public float maxHealth = 100f; // Maksimum can
    private float currentHealth; // �u anki can

    [Header("Stamina Ayarlar�")]
    public Slider staminaBar; // Stamina bar� referans�
    public float maxStamina = 100f; // Maksimum stamina
    private float currentStamina; // �u anki stamina
    public float staminaRegenRate = 5f; // Staminan�n yenilenme h�z�
    public float staminaCost = 10f; // Ko�ma s�ras�nda harcanan stamina
    private bool isMoving = false; // Hareket edip etmedi�ini kontrol et

    private Rigidbody rb;
    private bool isGround = true;
    private bool Bas�ld�M� = false;
    private int Ko�ma = 1; // 1 = normal, 2 = ko�ma

    [Header("Ko�ma Butonu")]
    public Image SpeedButtonImage; // Butonun image bile�eni
    public Sprite yurumeSprite; // Y�r�me sprite'�
    public Sprite kosmaSprite; // Ko�ma sprite'�

    void Start()
    {
        HaritaYap�c� = FindObjectOfType<HaritaYap�c�>();
        OyunucuAd�.text = photonView.Owner.NickName;

        rb = GetComponent<Rigidbody>();

        // Can ve stamina bar ayarlar�
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
        if (OyunucuAd�) OyunucuAd�.gameObject.SetActive(!aktif);
        if (staminaBar) staminaBar.gameObject.SetActive(aktif);
        if (healthBar) healthBar.gameObject.SetActive(aktif);
        if (EnvanterPaneli) EnvanterPaneli.gameObject.SetActive(aktif);
    }
    void Update()
    {
        if (!photonView.IsMine)
        {
            return; // Sadece ba�kas�n�n karakteri i�in ��k
        }

        if (HaritaYap�c�.Ba�lad�M�)
        {
            ShowLocalUI(true);
        }
        else if (!HaritaYap�c�.Ba�lad�M�)
        {
            ShowLocalUI(false);
        }
        // Aktif h�z� belirle
        float aktifH�z = h�z * Ko�ma;

        // Joystick girdileri
        float horizontal = FixedJoystick.Horizontal * aktifH�z;
        float vertical = FixedJoystick.Vertical * aktifH�z;

        // Kameran�n y�n�ne g�re hareket
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = playerCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 hareket = cameraForward * vertical + cameraRight * horizontal;

        // Hareket uygula
        rb.velocity = new Vector3(hareket.x, rb.velocity.y, hareket.z);

        // FOV de�i�imi
        float hedefFov = Ko�ma == 2 ? kosmaFov : normalFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, hedefFov, Time.deltaTime * fovDegismeH�z�);

        // Stamina yenileme
        if (currentStamina < maxStamina)
        {
            if (Ko�ma == 1)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
                staminaBar.value = currentStamina;
            }
        }

        // Ko�ma s�ras�nda stamina kayb� sadece hareket edildi�inde
        if (Ko�ma == 2 && isMoving && currentStamina > 0)
        {
            currentStamina -= staminaCost * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            staminaBar.value = currentStamina;

            // E�er stamina 0'a d��t�yse, y�r�me moduna ge�
            if (currentStamina <= 0)
            {
                Ko�ma = 1;
                SpeedButtonImage.sprite = yurumeSprite; // Ko�ma yerine y�r�me sprite'�n� g�ster
            }
        }

        // E�er hareket etmiyorsan�z, isMoving false olsun
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
        if (!photonView.IsMine && OyunucuAd� != null && Camera.main != null)
        {
            Vector3 direction = Camera.main.transform.position - OyunucuAd�.transform.position;
            direction.y = 0f; // Sadece Y ekseninde d�ns�n, yukar�-a�a�� bakmas�n
            if (direction.sqrMagnitude > 0.001f) // �ok yak�nda de�ilse
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                OyunucuAd�.transform.rotation = lookRotation;
            }
        }
    }

    public void Jump()
    {
        if (!photonView.IsMine) return;

        if (isGround)
        {
            rb.velocity = new Vector3(rb.velocity.x, z�plama, rb.velocity.z);
            isGround = false;
        }
    }

    public void speedButon()
    {
        Bas�ld�M� = !Bas�ld�M�;
        Ko�ma = Bas�ld�M� ? 2 : 1;

        // Ko�ma butonunun sprite'�n� de�i�tir
        if (Ko�ma == 2)
        {
            SpeedButtonImage.sprite = kosmaSprite; // Ko�ma modunda ko�ma sprite'� g�ster
        }
        else
        {
            SpeedButtonImage.sprite = yurumeSprite; // Y�r�me modunda y�r�me sprite'� g�ster
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
