using UnityEngine;
using UnityEngine.Events;
using Harborview.GameTools;
namespace Harborview.GameTools
{
    public class Interactable : MonoBehaviour
    {
        public bool isTarget;
        public bool WillsHeart;
        public Vector3 currSize;
        public Vector3 maxSize;
        public Vector3 minSize;
        
        //[HideInInspector] 
        public ZoneMarker ownerZone;
        //public UnityEvent onWrongGuess;
        //public UnityEvent onCorrectGuess;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            currSize = GetComponent<Transform>().localScale;
            if(!WillsHeart)
                maxSize = new Vector3(15f, 15f, 15f);
            minSize = new Vector3(1f, 1f, 1f);
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Grow()
        {
            if (currSize != maxSize) 
            {
                
                    currSize = new Vector3(currSize.x += 1, currSize.y += 1, currSize.z += 1);
                    transform.localScale = currSize;
            }
        }

        public void Shrink()
        {
            if (currSize != minSize)
            {

                currSize = new Vector3(currSize.x -= 1, currSize.y -= 1, currSize.z -= 1);
                transform.localScale = currSize;
            }
        }

        public void ReturnAnswer()
        {
            if (isTarget)
            {
                ownerZone.OnTargetFound();
            }
            else
            {
                Debug.Log(currSize);
                //OnWrongGuess();
            }

        }

        void OnWrongGuess()
        {
            ownerZone.WrongTarget();
        }
    }
}