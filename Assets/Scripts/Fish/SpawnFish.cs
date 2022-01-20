using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

/// <summary>
/// Unused class. Ignore for now.
/// </summary>
public class SpawnFish : MonoBehaviour
{

    public Vector3 center, size;
    public GameObject FishPrefab;

    public void SpawnAnimal()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2),
                                           Random.Range(-size.y / 2, size.y / 2),
                                           Random.Range(-size.z / 2, size.z / 2));

        Instantiate(FishPrefab, pos, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
}
