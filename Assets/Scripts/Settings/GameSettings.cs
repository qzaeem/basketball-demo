using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Basketball_Demo
{
    [CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings Asset", order = 1)]
    public class GameSettings : ScriptableObject
    {
        public List<GameRoundData> gameRounds;
        public int maxMissesAllowed = 10;
        public float basketMoveDurationMultiplier;

        [Header("Basketball")]
        public float maxThrowForceHorizontal;
        public float maxThrowForceVertical;
        public float maxThrowAngle = 90;
        public float minThrowAngle = 15;

        public GameRoundData GetNextRound(GameRound currentRound)
        {
            if(currentRound == 0)
            {
                return gameRounds[0];
            }

            int currentRoundIndex = gameRounds.IndexOf(gameRounds.FirstOrDefault(r => r.round == currentRound));

            return currentRoundIndex < 0 || currentRoundIndex >= gameRounds.Count - 1
                ? null : gameRounds[currentRoundIndex + 1];
        }
    }
}
