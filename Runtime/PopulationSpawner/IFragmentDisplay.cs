using UnityEngine;
using Harborview.GameTools;
namespace Harborview.GameTools
{
        public interface IFragmentDisplay
        {
            GameObject Panel { get; }
            void ClosePanel();
            void Unlock(StoryFragment fragment);
        }
}

