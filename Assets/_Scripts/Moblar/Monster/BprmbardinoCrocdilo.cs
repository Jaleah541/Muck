using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

namespace Ursaanimation.CubicFarmAnimals
{
    public class BombardinoCrocdiloAI : MonoBehaviour
    {
        public GameObject g�r��Alan�;

        private NavMeshAgent navMeshAgent;
        private Transform player;
        private bool i�inde = false;
        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Update()
        {
            if (i�inde)
            {
                navMeshAgent.SetDestination(player.position);
            }
            else if (!i�inde)
            {

            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                i�inde = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                i�inde = true;
            }
        }
    }
}
