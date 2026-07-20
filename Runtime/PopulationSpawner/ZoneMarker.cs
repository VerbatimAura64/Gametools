using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Harborview.GameTools;

namespace Harborview.GameTools
{
    public class ZoneMarker : MonoBehaviour
    {
        public PopulationPool pool;
        public int spawnPointMax;
        public Transform[] spawnPoints;
        [UnityEngine.Range(0.8f, 1f)] public float camouflageTightness;
        public StoryFragment fragment;
        public StoryFragment failFragment;
        public Vector3 planeSize;

        [HideInInspector] public List<GameObject> spawnedInstances = new();
        //[HideInInspector] 
        public bool resolved;
        private IFragmentDisplay fm;

        private void Awake()
        {
            fm = GameObject.FindGameObjectWithTag("GameController").GetComponent<IFragmentDisplay>();
        }

        public void OnTargetFound()
        {
            if (resolved) return;
            resolved = true;
            fm.Unlock(fragment);
        }

        public void WrongTarget()
        {
            fm.Unlock(failFragment);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
    }
}