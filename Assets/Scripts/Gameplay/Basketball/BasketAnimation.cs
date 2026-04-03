using DG.Tweening;
using UnityEngine;

namespace Basketball_Demo
{
    public class BasketAnimation : MonoBehaviour
    {
        [SerializeField] private Transform netTransform;
        [SerializeField] private float animationDuration;

        private Sequence animationSequence;

        public void PlayBasketAnimation()
        {
            animationSequence.Kill();

            animationSequence = DOTween.Sequence()
                .Append(netTransform.DOScaleY(1.1f, animationDuration / 4))
                .Append(netTransform.DOScaleY(0.9f, animationDuration / 3))
                .Append(netTransform.DOScaleY(1f, animationDuration / 4));
        }
    }
}
