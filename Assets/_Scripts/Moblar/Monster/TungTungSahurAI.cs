using UnityEngine;
using UnityEngine.AI;

namespace Ursaanimation.CubicFarmAnimals
{
    public class TungTungSahurAI : MonoBehaviour
    {
        public float saldýrýMesafesi = 2f;
        public float dönüþHýzý = 5f; // AI'nin oyuncuya dönme hýzý
        private NavMeshAgent navMeshAgent;
        private Transform player;
        private bool playerFound = false;

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (!playerFound)
            {
                FindClosestPlayer();
            }
            else
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (distanceToPlayer > saldýrýMesafesi)
                {
                    navMeshAgent.SetDestination(player.position);
                }
                else
                {
                    // Saldýrý mesafesinde dur
                    navMeshAgent.ResetPath();

                    // Oyuncuya dön
                    Vector3 direction = (player.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * dönüþHýzý);

                    // Saldýrý
                    Debug.Log("Saldýrýyor!");

                    // Buraya animasyon ekleyebilirsin:
                    // animator.SetTrigger("Attack");
                }
            }
        }

        private void FindClosestPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            if (players.Length > 0)
            {
                GameObject closestPlayer = null;
                float closestDistance = Mathf.Infinity;

                foreach (GameObject potentialPlayer in players)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, potentialPlayer.transform.position);

                    if (distanceToPlayer < closestDistance)
                    {
                        closestDistance = distanceToPlayer;
                        closestPlayer = potentialPlayer;
                    }
                }

                if (closestPlayer != null)
                {
                    player = closestPlayer.transform;
                    playerFound = true;
                    Debug.Log("En yakýn Player objesi bulundu: " + player.name);
                }
            }
            else
            {
                Debug.LogError("Player objesi bulunamadý!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player AI'nin içinde.");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player AI'nin dýþýna çýktý.");
            }
        }
    }
}
