using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class KarakterButonları : MonoBehaviourPunCallbacks
{
    public GameObject EnvaterMenu;
    public GameObject PauseMenu;
    private bool EnvanterMenuAcık = false;
    private bool GamePaused = false;
    

    void Start()
    {
        EnvaterMenu.SetActive(false);
        PauseMenu.SetActive(false);
    }

    void Update()
    {
        // Eğer pause işlemi yapılmışsa, bu UI'yi güncellemek için kod yazılabilir.
    }

    public void Envanter()
    {
        if (!EnvanterMenuAcık)
        {
            EnvaterMenu.SetActive(true);
            EnvanterMenuAcık = true;
        }
        else if (EnvanterMenuAcık)
        {
            EnvaterMenu.SetActive(false);
            EnvanterMenuAcık = false;
        }
    }

    public void Pause()
    {
        if (!GamePaused)
        {
            PauseMenu.SetActive(true);
            GamePaused = true;
        }
        else if (GamePaused)
        {
            PauseMenu.SetActive(false);
            GamePaused = false;
        }
    }

    public void ReturnMainMenu()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsConnected)
        {
            StartCoroutine(LeaveRoomAndLoadMenu());
        }
        else
        {
            Debug.Log("Zaten odada değilsiniz.");
            PhotonNetwork.LoadLevel("SampleScene");
        }
    }

    IEnumerator LeaveRoomAndLoadMenu()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("Odadan çıkılmaya çalışılıyor...");

        float timer = 0f;
        while (PhotonNetwork.InRoom)
        {
            timer += Time.deltaTime;
            if (timer > 5f) // 5 saniye bekledik, hala çıkamadıysa
            {
                Debug.LogWarning("Çıkış zaman aşımı! Zorla ana menüye dönülüyor.");
                break;
            }
            yield return null;
        }

        // Odayı terk ettikten sonra level yükleniyor
        PhotonNetwork.LoadLevel("SampleScene");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Başarıyla odadan çıkıldı!");

        // Odayı terk ettikten sonra yeni sahneye geçiş
        PhotonNetwork.LoadLevel("SampleScene");
    }



}
