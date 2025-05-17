using UnityEngine;
using UnityEngine.AI;

namespace Ursaanimation.CubicFarmAnimals
{
    public class SheepAI : MonoBehaviour
    {
        public Animator animator;
        public NavMeshAgent navMeshAgent;

        // Animasyon isimleri
        public string walkForwardAnimation = "walk_forward";
        public string walkBackwardAnimation = "walk_backwards";
        public string turn90LAnimation = "turn_90_L";
        public string turn90RAnimation = "turn_90_R";
        public string trotAnimation = "trot_forward";
        public string sittostandAnimation = "sit_to_stand";
        public string standtositAnimation = "stand_to_sit";
        public string idleAnimation = "idle";

        // Hareket parametreleri
        public float wanderRadius = 10f;
        public float wanderTime = 5f;
        private float timer;

        // Keçi ayakta mý?
        private bool isStanding = false;

        void Start()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            timer = wanderTime;
        }

        void Update()
        {
            Wander();
            HandleAnimations();
        }

        // Keçinin etrafta rastgele gezmesini saðlamak
        void Wander()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += transform.position;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
                {
                    navMeshAgent.SetDestination(hit.position);
                }

                timer = wanderTime; // Zamanlayýcýyý sýfýrla
            }
        }

        // Hareketlere baðlý olarak animasyonlarý tetikle
        void HandleAnimations()
        {
            if (navMeshAgent.velocity.magnitude > 0.1f)
            {
                animator.Play(walkForwardAnimation); // Sadece yürüme animasyonu
                isStanding = true;
            }
            else
            {
                if (!isStanding)
                {
                    animator.Play(sittostandAnimation); // Oturuyorsa sadece bir kez ayaða kalk
                    isStanding = true;
                }
                else
                {
                    animator.Play(idleAnimation); // Zaten ayaktaysa idle oynasýn
                }
            }
        }

        // Rastgele dönüþler
        public void TurnRandomly()
        {
            int randomTurn = Random.Range(0, 2); // 0: sola, 1: saða
            if (randomTurn == 0)
            {
                animator.Play(turn90LAnimation);
            }
            else
            {
                animator.Play(turn90RAnimation);
            }
        }
    }
}
