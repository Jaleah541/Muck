using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

namespace Ursaanimation.CubicFarmAnimals
{
    public class BombardinoCrocdiloAI : MonoBehaviour
    {
        public GameObject görüþAlaný;

        private NavMeshAgent navMeshAgent;
        private Transform player;
        private bool içinde = false;
        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Update()
        {
            if (içinde)
            {
                navMeshAgent.SetDestination(player.position);
            }
            else if (!içinde)
            {

            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                içinde = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                içinde = true;
            }
        }
    }
}
