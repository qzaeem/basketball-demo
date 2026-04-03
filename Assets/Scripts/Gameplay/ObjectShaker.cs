using DG.Tweening;
using UnityEngine;

namespace Basketball_Demo
{
    public class ObjectShaker : Singleton<ObjectShaker>
    {
        [SerializeField] private Transform shakeTarget;

        [Header("Shake props")]
        [SerializeField] private Vector3 strength = Vector3.one;
        [SerializeField] private int vibrato = 22;

        [Header("Additional shake settings")]
        [SerializeField] private float randomness = 90;
        [SerializeField] private bool snapping = false;
        [SerializeField] private bool fadeOut = true;
        [SerializeField] private ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Harmonic;

        private Vector3 originalLocalPosition;
        private Tween shakeTween;

        private void Start()
        {
            originalLocalPosition = shakeTarget.localPosition;
        }

        public void StartShake(float duration)
        {
            shakeTween?.Kill(true);
            shakeTween = shakeTarget.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut, randomnessMode)
                                    .OnComplete(() => shakeTarget.localPosition = originalLocalPosition);
        }
    }
}
