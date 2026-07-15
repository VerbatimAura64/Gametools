using UnityEngine;
using Harborview.GameTools;
namespace Harborview.GameTools
{
    public class TargetTell : MonoBehaviour
    {
        public Renderer tellRenderer;
        public Color obviousTellColor = Color.red;
        public Color normalColor = Color.white;

        static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");
        public void ApplyTightness(float tightness)
        {
            Color blended = Color.Lerp(obviousTellColor, normalColor, tightness);
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            tellRenderer.GetPropertyBlock(mpb);
            mpb.SetColor(ColorProperty, blended);
            tellRenderer.SetPropertyBlock(mpb);
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
//sharedM