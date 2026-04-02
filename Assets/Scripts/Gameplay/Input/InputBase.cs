using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Basketball_Demo.Gameplay
{
    public abstract class InputBase : MonoBehaviour
    {
        [SerializeField] protected float minSwipeTime = 0.1f;
        [SerializeField, Range(0, 1), Tooltip("(End position of the input / Max input value). For example, if set to 0.5 and using joystick, the minimum and maximum throw angles will correspond to 0 and 0.5 (magnitude) value of input vector of joystick respectively")] 
        protected float inputScaleForMaxAngle;

        protected CancellationTokenSource inputCTS;

        protected Vector2 inputStartPosition;
        protected Vector2 inputEndPosition;
        protected float inputStartTime;

        protected virtual void OnEnable()
        {
            EventManager.changeInputStatusEvent += ChangeInputStatus;
        }

        protected virtual void OnDisable()
        {
            EventManager.changeInputStatusEvent -= ChangeInputStatus;
        }

        protected virtual void ChangeInputStatus(bool isActive)
        {
            ReleaseInput();

            if (isActive)
            {
                ScanInput();
            }
            else
            {
                TaskUtils.CancelAndDisposeCTS(ref inputCTS);
            }
        }

        protected virtual void ReleaseInput()
        {
            inputStartPosition = Vector2.zero;
            inputEndPosition = Vector2.zero;
            inputStartTime = 0;
        }

        /// <summary>
        /// Should only be called once drag or a change in position is detected
        /// </summary>
        /// <param name="startPosition">The starting position of the input vector</param>
        protected virtual void OnInputStarted(Vector2 startPosition)
        {
            inputStartPosition = startPosition;
            inputStartTime = Time.unscaledTime;
        }

        /// <summary>
        /// Should only be called if the endPosition is not equal to the startPosition. The unscaledTime should be greater than the recorded inputStartTime as well.
        /// </summary>
        /// <param name="endPosition">The ending position of the input vector</param>
        /// <param name="maxInputValue">Maximum value of Y of input. For mouse, this can be equal to the diagonal vector from bottom-left of the screen to the top-right, and for joystick, this is equal to 1.0</param>
        protected virtual (float throwSpeed, float throwAngle, Vector2 direction) OnInputEnded(Vector2 endPosition, float maxInputValue)
        {
            Vector2 throwVector = endPosition - inputStartPosition; // The input value
            float elapsedTime = Time.unscaledTime - inputStartTime;
            float maxSpeed = maxInputValue / minSwipeTime;
            float throwSpeed = throwVector.magnitude / elapsedTime;
            float throwAngle = throwVector.magnitude / (maxInputValue * inputScaleForMaxAngle);

            ReleaseInput();

            return (Mathf.Clamp01(throwSpeed / maxSpeed), Mathf.Clamp01(throwAngle), throwVector.normalized);
        }

        protected virtual void OnInputUpdated(Vector2 endPosition, float maxInputValue)
        {
            // Implement if required
        }

        protected abstract UniTask ScanInput();
    }
}
