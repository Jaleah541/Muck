using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class MobilKameraKontrol : MonoBehaviourPun
{
    public float donusHizi = 0.2f;
    public List<RectTransform> aktifUIAlanlari;
    public Transform karakterTransform;
    public GameObject karakter;

    private float currentXRotation = 0f;
    public float minYukari = -60f;
    public float maxAsagi = 60f;

    private List<int> kontrolEdilenParmakIDleri = new List<int>();

    private SkinnedMeshRenderer[] karakterRendererlar;

    void Start()
    {
        // Baþlangýçta Renderer bileþenlerini alýyoruz
        karakterRendererlar = karakter.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (!photonView.IsMine)
        {
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            // Diðer oyunculara: karakter görünmesin, sadece kol görünsün
            foreach (SkinnedMeshRenderer r in karakterRendererlar)
                r.enabled = true;

        }

        // Kendimize: karakter gözüksün, kol görünmesin
        foreach (SkinnedMeshRenderer r in karakterRendererlar)
            r.enabled = false;


        // Kol ve karakteri ayný pozisyonda tutuyoruz
        //kolTrans.position = karakterTransform.position;
      //  kolTrans.rotation = karakterTransform.rotation;

        // Kamera kontrolü
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch dokunus = Input.GetTouch(i);

            switch (dokunus.phase)
            {
                case TouchPhase.Began:
                    if (IsPointerOverAnyUIElement(dokunus.position))
                    {
                        kontrolEdilenParmakIDleri.Add(dokunus.fingerId);
                    }
                    break;

                case TouchPhase.Moved:
                    if (kontrolEdilenParmakIDleri.Contains(dokunus.fingerId))
                    {
                        Vector2 fark = dokunus.deltaPosition;
                        float yatayDonus = fark.x * donusHizi;
                        float dikeyDonus = -fark.y * donusHizi;

                        // Yatay dönüþ karakteri döndürür
                        karakterTransform.Rotate(0, yatayDonus, 0, Space.World);

                        // Dikey dönüþ sadece kamerayý eðer
                        currentXRotation += dikeyDonus;
                        currentXRotation = Mathf.Clamp(currentXRotation, minYukari, maxAsagi);

                        transform.localRotation = Quaternion.Euler(currentXRotation, 0, 0);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    kontrolEdilenParmakIDleri.Remove(dokunus.fingerId);
                    break;
            }
        }
    }

    private bool IsPointerOverAnyUIElement(Vector2 pozisyon)
    {
        foreach (RectTransform uiElement in aktifUIAlanlari)
        {
            if (uiElement != null && RectTransformUtility.RectangleContainsScreenPoint(uiElement, pozisyon, null))
            {
                return true;
            }
        }
        return false;
    }
}
