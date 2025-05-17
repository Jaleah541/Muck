using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class AllMenuController : MonoBehaviourPunCallbacks
{
    public static string ras;

    [Header("Menüler")]
    public GameObject NameMenu;
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public GameObject AchievementsMenu;
    public GameObject FriendsMenu;
    public GameObject LoadingMenu;
    [Header("Hata Paneli")]
    public GameObject JoinhataPaneli;
    public GameObject HosthataPaneli;
    [Header("UI'lar")]
    public TMP_Text ProfilTExt;
    public TMP_Text HataText;
    public TMP_InputField LOBBYID;
    public TMP_InputField NameInput;
    public TMP_Text LoadingText;
    public TMP_InputField genişlikInput;
    public TMP_InputField yükseklikInput;
    public Toggle tamEkranToggle;

    [Header("Prefablar")]
    public GameObject hostPrefab;
    List<RoomInfo> roomList = new List<RoomInfo>();

    private bool isConnecting = false;

    void Start()
    {
        Resolution ekran = Screen.currentResolution;
        Screen.SetResolution(ekran.width, ekran.height, FullScreenMode.FullScreenWindow, ekran.refreshRateRatio);

        // Show the loading menu when starting the connection
        LoadingMenu.SetActive(true);
        LoadingText.text = "Loading";

        isConnecting = true;
        StartCoroutine(UpdateLoadingText());

        if (PlayerPrefs.HasKey("Username"))
        {
            // Kullanıcı daha önce isim girmiş
            string savedName = PlayerPrefs.GetString("Username");
            PhotonNetwork.NickName = savedName;
            ProfilTExt.text = savedName;
            NameMenu.SetActive(false);
            MainMenu.SetActive(true);
        }
        else
        {
            // İlk kez giriş yapıyor
            NameMenu.SetActive(true);
            MainMenu.SetActive(false);
        }

        FriendsMenu.SetActive(false);
        HosthataPaneli.SetActive(false);
        JoinhataPaneli.SetActive(false);
        SettingsMenu.SetActive(false);
        AchievementsMenu.SetActive(false);
        PhotonNetwork.GameVersion = "1.0";
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Bağlantı başlatılır
        }
        else
        {
            Debug.Log("Zaten bağlı, yeniden bağlanmaya gerek yok.");
        }
    }

    IEnumerator UpdateLoadingText()
    {
        string loadingMessage = "Loading";
        int dotCount = 0;

        while (isConnecting)
        {
            // Add a dot every 0.3 seconds
            LoadingText.text = loadingMessage + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // Reset to 0 after 3 dots
            yield return new WaitForSeconds(0.3f);
        }

        // Reset the loading text when done
        LoadingText.text = "Loading";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Sunucuya Bağlanıldı");
        // Stop the loading text update and hide the loading menu
        isConnecting = false;
        LoadingMenu.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Bağlanma başarısız oldu: " + cause);
        // Stop the loading text update and hide the loading menu
        isConnecting = false;
        LoadingMenu.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        MainMenu.SetActive(false);
        Debug.Log("Odaya girildi!");
        PhotonNetwork.Instantiate(hostPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Odaya giriş başarısız: " + message);
        JoinhataPaneli.SetActive(true);
    }

    // Existing methods (e.g., NameOKay, Profil, HostLobby, JoinLobby, etc.)
    public void NameOKay()
    {
        if (string.IsNullOrEmpty(NameInput.text) || string.IsNullOrWhiteSpace(NameInput.text))
        {
            Debug.Log(NameInput.text + "Uygun bir isim değil");
            HataText.text = "Lütfen tam ve boşluksuz bir isim yazınız";
            return;
        }

        PhotonNetwork.NickName = NameInput.text;
        PlayerPrefs.SetString("Username", NameInput.text); // ⭐ İsmi kaydet
        PlayerPrefs.Save(); // Değişikliği diske yaz

        MainMenu.SetActive(true);
        NameMenu.SetActive(false);
    }

    public void Profil()
    {
        MainMenu.SetActive(false);
        NameMenu.SetActive(true);

        if (PlayerPrefs.HasKey("Username"))
        {
            NameInput.text = PlayerPrefs.GetString("Username");
            ProfilTExt.text = PlayerPrefs.GetString("Username");
        }
    }

    public void HostLobby()
    {
        if (PhotonNetwork.IsConnected)
        {
            string rastgeleID = "";
            bool benzersizIDBulundu = false;

            // En fazla 20 kez denesin, güvenlik önlemi
            for (int i = 0; i < 20; i++)
            {
                int rastgeleSayi = Random.Range(10, 99);
                string olasiID = rastgeleSayi.ToString();

                bool ayniOdaVar = false;
                foreach (RoomInfo room in roomList)
                {
                    if (room.Name == olasiID && !room.RemovedFromList)
                    {
                        ayniOdaVar = true;
                        break;
                    }
                }

                if (!ayniOdaVar)
                {
                    rastgeleID = olasiID;
                    benzersizIDBulundu = true;
                    break;
                }
            }

            if (benzersizIDBulundu)
            {
                ras = rastgeleID;
                RoomOptions options = new RoomOptions();
                options.MaxPlayers = 6;

                PhotonNetwork.CreateRoom(rastgeleID, options, TypedLobby.Default);
                Debug.Log("Oda oluşturuluyor: " + rastgeleID);
            }
            else
            {
                Debug.Log("Uygun boş bir ID bulunamadı.");
                HosthataPaneli.SetActive(true);
            }
        }
        else
        {
            HosthataPaneli.SetActive(true);
        }
    }

    public void JoinLobby()
    {
        if (string.IsNullOrEmpty(LOBBYID.text) || !PhotonNetwork.IsConnected || string.IsNullOrWhiteSpace(LOBBYID.text))
        {
            JoinhataPaneli.SetActive(true);
            return;
        }

        PhotonNetwork.JoinRoom(LOBBYID.text);
    }

    public void Settings()
    {
        SettingsMenu.SetActive(true);
        MainMenu.SetActive(false);
    }
    public void ReturnMainMenu()
    {
        MainMenu.SetActive(true);
        SettingsMenu.SetActive(false);
        AchievementsMenu.SetActive(false);
    }
    public void Achievements()
    {
        AchievementsMenu.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void Okay()
    {
        HosthataPaneli.SetActive(false);
        JoinhataPaneli.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void inviteFriends()
    {
        Application.OpenURL("");
    }

    public void friendsMenu()
    {
        MainMenu.SetActive(false);
        FriendsMenu.SetActive(true);
    }

    public void ReturnFriendsMenu()
    {
        MainMenu.SetActive(true);
        FriendsMenu.SetActive(false);
    }
    public void Ayarla()
    {
        int genişlik = int.Parse(genişlikInput.text);
        int yükseklik = int.Parse(yükseklikInput.text);

        bool tamEkran = tamEkranToggle.isOn;

        Çözünürlük(genişlik, yükseklik, tamEkran);
    }
    public void Çözünürlük(int Genişlik,int yükseklik, bool tamekran)
    {
        Screen.SetResolution(Genişlik,yükseklik,tamekran);
    }
}
