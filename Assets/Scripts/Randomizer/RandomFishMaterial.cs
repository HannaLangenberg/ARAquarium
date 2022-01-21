using UnityEngine;
using Random = UnityEngine.Random;

namespace Randomizer
{
    /// <summary>
    /// Class <c>RandomFishMaterial</c> is a component on the discus prefab.
    /// </summary>
    public class RandomFishMaterial : MonoBehaviour
    {
        public Material[] materials;

        void Start()
        {
            if (materials.Length > 0)
            {
                ApplyMaterial();
            }
        }
        
        /// <summary>
        /// For each discus instance a random material is chosen from the materials list.   
        /// </summary>
        void ApplyMaterial()
        {
            GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
        }
    }
}