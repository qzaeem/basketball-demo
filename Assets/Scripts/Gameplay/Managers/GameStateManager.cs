using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Basketball_Demo.Gameplay
{
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField] private Transform basketsParent;
        [SerializeField] private List<Transform> baskets;
        [SerializeField] private float basketMoveDuration;

        private GameRoundData currentRoundData;
        private CancellationTokenSource startRoundCTS;
        private Sequence movementTween;

        public bool HasGameEnded {  get; private set; }
        public int GameScore { get; private set; }
        public int ShotsMissed {  get; private set; }

        private void OnEnable()
        {
            EventManager.madeBasketEvent += OnMadeBasket;
            EventManager.missedBasketEvent += OnMissedBasket;
        }

        private void OnDisble()
        {
            EventManager.madeBasketEvent-= OnMadeBasket;
            EventManager.missedBasketEvent -= OnMissedBasket;
        }

        private void Start()
        {
            GameScore = 0;
            ShotsMissed = 0;
            HasGameEnded = false;
            currentRoundData = new();
            currentRoundData.round = GameRound.None;
            StartRoundLoop().Forget();
        }

        private Vector3[] GetCirclePositions(float radius, int count)
        {
            Vector3[] positions = new Vector3[count];

            float angleStep = Mathf.PI * 2f / count; // full circle divided by count

            for (int i = 0; i < count; i++)
            {
                float angle = i * angleStep;

                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                positions[i] = new Vector3(x, 0f, z);
            }

            return positions;
        }

        private void RevolvingBaskets(bool firstTime)
        {
            Vector3[] targetPositions = GetCirclePositions(currentRoundData.distanceFromParent, baskets.Count);
            Quaternion[] initialRotations = new Quaternion[baskets.Count];

            for (int i = 0; i < baskets.Count; i++)
            {
                initialRotations[i] = baskets[i].rotation;
            }

            movementTween = DOTween.Sequence();

            if (firstTime)
            {
                movementTween.Append(basketsParent.DOMove(currentRoundData.parentPosition, basketMoveDuration));

                for (int i = 0; i < baskets.Count; i++)
                {
                    movementTween.Join(
                        baskets[i]
                            .DOLocalMove(targetPositions[i], basketMoveDuration)
                            .SetEase(Ease.OutSine)
                    );
                }
            }

            movementTween.Append(
                basketsParent
                    .DOLocalRotate(new Vector3(0, 360f, 0), basketMoveDuration * 5, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart)
                    .OnUpdate(() =>
                    {
                        for (int i = 0; i < baskets.Count; i++)
                        {
                            baskets[i].rotation = initialRotations[i];
                        }
                    })
               );
        }

        private async UniTask StartRoundLoop()
        {
            CancellationToken token = TaskUtils.RenewCTS(ref startRoundCTS);

            while (true)
            {
                GameRoundData nextRound = GameSettingsProvider.Instance.GameSettings.GetNextRound(currentRoundData.round);
                movementTween.Kill();
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.newRound);
                ShotsMissed = 0;
                EventManager.InvokeGameStateUpdateEvent();

                if (nextRound == null || nextRound.round == currentRoundData.round) // When the player reaches last round, it loops indefinitely until the player has lost the game
                {
                    basketMoveDuration *= GameSettingsProvider.Instance.GameSettings.basketMoveDurationMultiplier;
                    int targetScore = GameScore + currentRoundData.roundScore;
                    RevolvingBaskets(false);
                    await UniTask.WaitUntil(() => GameScore >= targetScore, cancellationToken: token);
                    continue;
                }

                currentRoundData = nextRound;

                switch(currentRoundData.round)
                {
                    case GameRound.StationaryBaskets:
                        movementTween = DOTween.Sequence()
                            .Append(basketsParent.DOMove(currentRoundData.parentPosition, basketMoveDuration));

                        for (int i = 0; i < baskets.Count; i++)
                        {
                            movementTween.Join(baskets[i].DOLocalMove(currentRoundData.basketLocalPositions[i], basketMoveDuration));
                        }
                        break;
                    case GameRound.DistanceBaskets:
                        movementTween = DOTween.Sequence()
                            .Append(basketsParent.DOMove(currentRoundData.parentPosition, basketMoveDuration));
                        break;
                    case GameRound.MovingBaskets:
                        movementTween = DOTween.Sequence();

                        for (int i = 0; i < baskets.Count; i++)
                        {
                            movementTween
                                .Join(
                                baskets[i].DOLocalMove(currentRoundData.basketLocalPositions[i], basketMoveDuration)
                                .SetLoops(-1, LoopType.Yoyo)
                                );
                        }
                        break;
                    case GameRound.RevolvingBaskets:
                        RevolvingBaskets(true);
                        break;
                    default:
                        break;
                }

                int roundTarget = GameScore + currentRoundData.roundScore;
                await UniTask.WaitUntil(() => GameScore >= roundTarget, cancellationToken: token);
            }
        }

        public void OnMadeBasket()
        {
            if (HasGameEnded)
            {
                return;
            }

            GameScore += currentRoundData.pointsPerBasket;
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.scoreBasket);
            EventManager.InvokeGameStateUpdateEvent();
        }

        public void OnMissedBasket() 
        {
            if (HasGameEnded)
            {
                return;
            }

            ShotsMissed += 1;
            EventManager.InvokeGameStateUpdateEvent();

            if(ShotsMissed >= GameSettingsProvider.Instance.GameSettings.maxMissesAllowed)
            {
                EventManager.InvokeGameEndEvent();
                EventManager.InvokeInputStatusEvent(false);
                ShotsMissed = GameSettingsProvider.Instance.GameSettings.maxMissesAllowed;
                HasGameEnded = true;
            }
        }
    }
}
