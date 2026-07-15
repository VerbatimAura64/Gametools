using UnityEngine;
using Harborview.GameTools;
namespace Harborview.GameTools
{
    [CreateAssetMenu(fileName = "PopulationPool", menuName = "HideAndSeek/Population Pool", order = 1)]
    public class PopulationPool : ScriptableObject
    {
        [System.Serializable]
        public struct WeightedPrefab
        {
            public GameObject prefab;
            public float weight;
        }


        public WeightedPrefab[] decoyPrefabs;
        public GameObject targetPrefab;

    }
}
