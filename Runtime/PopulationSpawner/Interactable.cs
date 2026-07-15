using UnityEngine;
using UnityEngine.Events;
using Harborview.GameTools;
namespace Harborview.GameTools
{
    public class Interactable : MonoBehaviour
    {
        public bool isTarget;
        //[HideInInspector] 
        public ZoneMarker ownerZone;
        //public UnityEvent onWrongGuess;
        //public UnityEvent onCorrectGuess;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ReturnAnswer()
        {
            if (isTarget)
            {
                ownerZone.OnTargetFound();
            }
            else
            {
                OnWrongGuess();
            }

        }

        void OnWrongGuess()
        {
            ownerZone.WrongTarget();
        }
    }
}