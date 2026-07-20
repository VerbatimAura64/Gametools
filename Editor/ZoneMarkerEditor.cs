using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Harborview.GameTools;
namespace Harborview.GameTools
{
    [CustomEditor(typeof(ZoneMarker))]
    public class ZoneMarkerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ZoneMarker zone = (ZoneMarker)target;
            int spawnNum = zone.spawnPointMax;
            Vector3 planeSize = zone.planeSize;
            if (GUILayout.Button("Create and Populate Spawns"))
            {
                CreateSpawns(zone, spawnNum, planeSize);
            }
            if (GUILayout.Button("Clear Spawns"))
            {
                ClearSpawns(zone);
            }
            if (GUILayout.Button("Populate"))
            {
                SpawnObjects(zone);
            }
            if (GUILayout.Button("Clear"))
            {
                ClearObjects(zone);
            }

        }

        private void CreateSpawns(ZoneMarker zone, int spawnNum, Vector3 planeSize)
        {
            ClearSpawns(zone);
            Vector3 plane = planeSize;
            zone.spawnPoints = new Transform[spawnNum];


            for (int i = 0; i < spawnNum; i++)
            {
                float randomX = Random.Range(-plane.x / 2, plane.x / 2);
                float randomY = Random.Range(-plane.z / 2, plane.z / 2);
                Vector3 spawnPosition = new Vector3(randomX, 0, randomY);
                Transform instance = new GameObject("Spawnpoint").transform;
                instance.position = spawnPosition;
                instance.parent = zone.transform;
                zone.spawnPoints[i] = instance;
            }
            SpawnObjects(zone);
        }

        private void ClearSpawns(ZoneMarker zone)
        {
            foreach (Transform instance in zone.spawnPoints)
            {
                if (instance != null) DestroyImmediate(instance.gameObject);
            }
            zone.spawnPoints = new Transform[0];
        }
        private void SpawnObjects(ZoneMarker zone)
        {
            //ClearObjects(zone);
            int targetIndex = Random.Range(0, zone.spawnPoints.Length);

            for (int i = 0; i < zone.spawnPoints.Length; i++)
            {
                GameObject prefab = (i == targetIndex)
                    ? zone.pool.targetPrefab : GetWeightedDecoy(zone.pool);
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, zone.transform);
                TargetTell tell = instance.GetComponent<TargetTell>();
                tell?.ApplyTightness(zone.camouflageTightness);
                instance.transform.position = zone.spawnPoints[i].position;
                instance.transform.rotation = zone.spawnPoints[i].rotation;


                Interactable interactable = instance.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.isTarget = (i == targetIndex);
                    interactable.ownerZone = zone;
                }

                zone.spawnedInstances.Add(instance);
            }
        }
        private void ClearObjects(ZoneMarker zone)
        {
            foreach (var instance in zone.spawnedInstances)
                if (instance != null) DestroyImmediate(instance);
            zone.spawnedInstances.Clear();
        }

        GameObject GetWeightedDecoy(PopulationPool pool)
        {
            float totalWeight = 0f;
            foreach (var decoy in pool.decoyPrefabs)
                totalWeight += decoy.weight;
            float randomValue = Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;
            foreach (var decoy in pool.decoyPrefabs)
            {
                cumulativeWeight += decoy.weight;
                if (randomValue <= cumulativeWeight)
                    return decoy.prefab;
            }
            return null; // Fallback in case of rounding errors
        }

    }
}
