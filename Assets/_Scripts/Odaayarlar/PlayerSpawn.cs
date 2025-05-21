using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    [Header("Spawn Pozisyonlar�")]
    public Transform[] spawnPositions;
    public GameObject playerPrefab;

    void Start()
    {
        // Sadece kendi oyuncun i�in spawn i�lemi yap�lmal�
        SpawnLocalPlayer();
    }

    private void SpawnLocalPlayer()
    { 
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        // Pozisyon yeterliyse o pozisyonda spawn et
        if (playerIndex < spawnPositions.Length)
        {
            Transform spawnPoint = spawnPositions[playerIndex];
            GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

            // Tag atama
            playerObject.tag = "Player";
            Debug.Log("Local player spawn edildi: " + PhotonNetwork.NickName);
        }
        else
        {
            // E�er pozisyon yetersizse rastgele bir pozisyonda spawn et
            int randomIndex = Random.Range(0, spawnPositions.Length);
            Transform spawnPoint = spawnPositions[randomIndex];
            GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

            // Tag atama
            playerObject.tag = "Player";
            Debug.Log("Yetersiz pozisyon � rastgele spawn edildi.");
        }
    }
}
