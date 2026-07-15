using UnityEngine;

namespace Harborview.GameTools
{
    public interface IGameState
    {
        bool IsPaused { get; }
        void PauseGame();
    }
}
