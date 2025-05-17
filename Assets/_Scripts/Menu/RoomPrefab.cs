using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class RoomPrefab : MonoBehaviourPunCallbacks
{
    public TMP_Text[] Oyuncuisimleri;
    public GameObject[] KickButon;
    public TMP_Text LobbyID;
    private AllMenuController menuController;
    public TMP_Text EasyText;
    public TMP_Text NormalText;
    public TMP_Text GamerText;
    public TMP_Text OnText;
    public TMP_Text OffText;
    public TMP_Text SurvivalText;
    public TMP_Text VErsusText;
    public TMP_Text CreativeText;
    public TMP_InputField SeedÝsmi;

    void Start()
    {
        Debug.Log("Ben master mýyým? " + PhotonNetwork.IsMasterClient);

        menuController = FindObjectOfType<AllMenuController>();

        if (LobbyID != null)
        {
            LobbyID.text = AllMenuController.ras;
        }
        else
        {
            Debug.LogError("LobbyID, Inspector'da atanmadý!");
        }

        // Oyuncu isimlerini güncelle
        UpdatePlayerList();
    }
    private void Awake()
    {
        // Sahne senkronizasyonunu aktif et
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void UpdatePlayerList()
    {
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            if (i < Oyuncuisimleri.Length)
            {
                Oyuncuisimleri[i].text = players[i].NickName;
                KickButon[i].SetActive(true);
            }
        }

        // Boþ slotlarý gizleyelim
        for (int i = players.Length; i < Oyuncuisimleri.Length; i++)
        {
            Oyuncuisimleri[i].text = "";
            KickButon[i].SetActive(false);
        }
    }

    public void Easy()
    {
        Debug.Log("Zorluk: Easy");
        // GameSettings.zorluk = "Easy";
        EasyText.color = Color.green;
        NormalText.color = Color.black;
        GamerText.color = Color.black;
    }

    public void Normal()
    {
        Debug.Log("Zorluk: Normal");
        // GameSettings.zorluk = "Normal";
        EasyText.color = Color.black;
        NormalText.color = Color.green;
        GamerText.color = Color.black;
    }

    public void Gamer()
    {
        Debug.Log("Zorluk: Gamer");
        // GameSettings.zorluk = "Gamer";
        EasyText.color = Color.black;
        NormalText.color = Color.black;
        GamerText.color = Color.green;
    }

    public void PlayerdamageON()
    {
        Debug.Log("Player Damage: ON");
        // GameSettings.playerDamage = true;
        OffText.color = Color.black;
        OnText.color = Color.green;
    }

    public void PlayerDamageOFF()
    {
        Debug.Log("Player Damage: OFF");
        // GameSettings.playerDamage = false;
        OffText.color = Color.green;
        OnText.color = Color.black;
    }

    public void Survival()
    {
        Debug.Log("Oyun Modu: Survival");
        // GameSettings.mode = "Survival";
        SurvivalText.color = Color.green;
        VErsusText.color = Color.black;
        CreativeText.color = Color.black;
    }

    public void Versus()
    {
        Debug.Log("Oyun Modu: Versus");
        // GameSettings.mode = "Versus";
        SurvivalText.color = Color.black;
        VErsusText.color = Color.green;
        CreativeText.color = Color.black;
    }

    public void Creative()
    {
        Debug.Log("Oyun Modu: Creative");
        // GameSettings.mode = "Creative";
        SurvivalText.color = Color.black;
        VErsusText.color = Color.black;
        CreativeText.color = Color.green;
    }
    // Master Client tarafýndan kick iþlemi
    public void Kick(int playerIndex)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Oyuncuyu bul
            Player playerToKick = PhotonNetwork.PlayerList[playerIndex];

            // RPC ile oyuncuya kendini odadan atma sinyali gönder
            photonView.RPC("ForceLeaveRoom", playerToKick);
            Debug.Log("Oyuncuya atýlma sinyali gönderildi: " + playerToKick.NickName);
        }
        else
        {
            Debug.LogError("Sadece Master Client kick yapabilir!");
        }
    }

    // Oyuncunun kendini odadan atmasý için kullanýlan RPC
    [PunRPC]
    public void ForceLeaveRoom()
    {
        Debug.Log("Sunucudan atýldýnýz.");
        PhotonNetwork.LeaveRoom();  // Bu komut ile oyuncuyu odadan çýkarýyoruz

        if (menuController != null)
        {
            menuController.MainMenu.SetActive(true); // Ana menüyü göster
        }
    }
    public GameObject hostPrefab; // Inspector’dan atamalýsýn

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UpdatePlayerList();
        Debug.Log("Oyuncu katýldý: " + newPlayer.NickName);

        // Sadece Master Client prefab oluþturmalý
        if (PhotonNetwork.IsMasterClient && GameObject.FindWithTag("Host") == null)
        {
            GameObject go = PhotonNetwork.Instantiate(hostPrefab.name, Vector3.zero, Quaternion.identity);
            Debug.Log("Host prefab instantiated by Master Client.");
        }
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdatePlayerList();  // Oyuncu ayrýldýðýnda listeyi güncelle
        Debug.Log("Oyuncu ayrýldý: " + otherPlayer.NickName);
    }

    public void Back()
    {
        PhotonNetwork.LeaveRoom();
        Destroy(gameObject);

        if (menuController != null)
        {
            menuController.MainMenu.SetActive(true);
        }
        else
        {
            Debug.LogError("AllMenuController bulunamadý!");
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string seedText = SeedÝsmi.text;

            // Eðer boþsa random üret
            if (string.IsNullOrEmpty(seedText))
            {
                seedText = Random.Range(100000, 999999).ToString();
            }

            // Oda özelliði olarak sakla
            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
            roomProps["seed"] = seedText;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

            Debug.Log("Oyun baþlatýlýyor... Seed: " + seedText);

            PhotonNetwork.LoadLevel("GameScene");
        }
    }

}