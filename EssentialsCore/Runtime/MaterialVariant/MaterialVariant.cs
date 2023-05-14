using UnityEngine;

namespace Essentials.Core
{
    public class MaterialVariant : MonoBehaviour
    {
        public int materialIndex = 0;
        public Color color = Color.white;
        private Renderer rend;

        private void Awake()
        {
            rend = GetComponent<Renderer>();

            Material material = rend.materials[materialIndex];
            material.color = color;
        }
    }
}