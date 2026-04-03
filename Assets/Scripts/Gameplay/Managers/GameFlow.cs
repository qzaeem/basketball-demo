using System.Collections.Generic;
using UnityEngine;

namespace Basketball_Demo.Gameplay
{
    public class GameFlow : Singleton<GameFlow>
    {
        [SerializeField] private GameStateManager gameState;
        [SerializeField] private Transform basketballContainer;

        private List<BasketballController> basketballPool = new();
        private int ballInUse;

        public GameStateManager GameState => gameState;

        private void OnEnable()
        {
            EventManager.spawnBallEvent += SpawnBasketball;
        }

        private void OnDisable()
        {
            EventManager.spawnBallEvent -= SpawnBasketball;
        }

        private void Start()
        {
            PopulateBasketballPool();
            EventManager.InvokeInputStatusEvent(true);
        }

        private void PopulateBasketballPool()
        {
            foreach(Transform bbt in basketballContainer)
            {
                BasketballController controller = bbt.GetComponent<BasketballController>();
                controller.InitializeBasketball();
                bbt.gameObject.SetActive(false);
                basketballPool.Add(controller);
            }

            ballInUse = -1;
            SpawnBasketball();
        }

        private void SpawnBasketball()
        {
            ballInUse++;

            if(ballInUse >= basketballPool.Count)
            {
                ballInUse = 0;
            }

            BasketballController ball = basketballPool[ballInUse];
            ball.gameObject.SetActive(true);
            ball.InitializeBasketball();
        }

        protected override void OnDestroy()
        {
            EventManager.ClearAllEvents();
            base.OnDestroy();
        }
    }
}
