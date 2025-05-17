using UnityEngine;
using UnityEngine.AI;

namespace Ursaanimation.CubicFarmAnimals
{
    public class TungTungSahurAI : MonoBehaviour
    {
        public float sald�r�Mesafesi = 2f;
        public float d�n��H�z� = 5f; // AI'nin oyuncuya d�nme h�z�
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

                if (distanceToPlayer > sald�r�Mesafesi)
                {
                    navMeshAgent.SetDestination(player.position);
                }
                else
                {
                    // Sald�r� mesafesinde dur
                    navMeshAgent.ResetPath();

                    // Oyuncuya d�n
                    Vector3 direction = (player.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * d�n��H�z�);

                    // Sald�r�
                    Debug.Log("Sald�r�yor!");

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
                    Debug.Log("En yak�n Player objesi bulundu: " + player.name);
                }
            }
            else
            {
                Debug.LogError("Player objesi bulunamad�!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player AI'nin i�inde.");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player AI'nin d���na ��kt�.");
            }
        }
    }
}
