using System;
using UnityEngine;

namespace Basketball_Demo
{
    public static class EventManager
    {
        #region Events
        public static event Action<bool> changeInputStatusEvent; 
        public static event Action spawnBallEvent;

        public static event Action<(float speed, float angle, Vector2 direction)> throwBallEvent;
        #endregion 

        #region Invokations
        public static void InvokeThrowBallEvent((float, float, Vector2) arg) => throwBallEvent?.Invoke(arg);
        public static void InvokeSpawnBallEvent() => spawnBallEvent?.Invoke();
        public static void InvokeInputStatusEvent(bool isActive) => changeInputStatusEvent?.Invoke(isActive);
        #endregion
    }
}
