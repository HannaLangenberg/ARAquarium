using UnityEngine;
using Random = UnityEngine.Random;

namespace Randomizer
{
    public class RandomFishMaterial : MonoBehaviour
    {
        public Material[] materials;

        // Start is called before the first frame update
        void Start()
        {
            if (materials.Length > 0)
            {
                ApplyMaterial();
            }
        }

        void ApplyMaterial()
        {
            GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
        }
    }
}