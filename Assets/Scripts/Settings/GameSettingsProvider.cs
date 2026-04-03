using UnityEngine;

namespace Basketball_Demo
{
    public class GameSettingsProvider : Singleton<GameSettingsProvider>
    {
        [SerializeField] private GameSettings gameSettings;

        public GameSettings GameSettings => gameSettings;
    }
}
