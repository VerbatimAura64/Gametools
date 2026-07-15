using UnityEngine;
using Harborview.GameTools;
namespace Harborview.GameTools
{
    [CreateAssetMenu(fileName = "StoryFragment", menuName = "HideAndSeek/Story Fragment")]
    public class StoryFragment : ScriptableObject
    {
        public int orderIndex;
        [TextArea] public string fragmentText;
        public AudioClip fragmentAudio;
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
