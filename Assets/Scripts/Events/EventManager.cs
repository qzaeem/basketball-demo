using System;
using UnityEngine;

namespace Basketball_Demo
{
    public static class EventManager
    {
        #region Events
        public static event Action<bool> changeInputStatusEvent; 
        public static event Action spawnBallEvent;
        public static event Action madeBasketEvent;
        public static event Action missedBasketEvent;
        public static event Action gameStateUpdateEvent;
        public static event Action gameEndEvent;

        public static event Action<(float speed, float angle, Vector2 direction)> updateBallInputEvent;
        public static event Action<(float speed, float angle, Vector2 direction)> throwBallEvent;
        #endregion 

        public static void ClearAllEvents()
        {
            changeInputStatusEvent = null;
            spawnBallEvent = null;
            madeBasketEvent = null;
            missedBasketEvent = null;
            gameStateUpdateEvent = null;
            gameEndEvent = null;
            updateBallInputEvent = null;
            throwBallEvent = null;
        }

        #region Invokations
        public static void InvokeThrowBallEvent((float, float, Vector2) arg) => throwBallEvent?.Invoke(arg);
        public static void InvokeUpdateBallInputEvent((float, float, Vector2) arg) => updateBallInputEvent?.Invoke(arg);
        public static void InvokeInputStatusEvent(bool isActive) => changeInputStatusEvent?.Invoke(isActive);
        public static void InvokeSpawnBallEvent() => spawnBallEvent?.Invoke();
        public static void InvokeMadeBasketEvent() => madeBasketEvent?.Invoke();
        public static void InvokeMissedBasketEvent() => missedBasketEvent?.Invoke();
        public static void InvokeGameStateUpdateEvent() => gameStateUpdateEvent?.Invoke();
        public static void InvokeGameEndEvent() => gameEndEvent?.Invoke();
        #endregion
    }
}
