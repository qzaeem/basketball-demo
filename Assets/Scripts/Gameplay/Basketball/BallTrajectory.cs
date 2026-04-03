using UnityEngine;
using UnityEngine.UI;

namespace Basketball_Demo.Gameplay
{
    public class BallTrajectory : MonoBehaviour
    {
        [SerializeField] private LineRenderer trajectoryLine;
        [SerializeField] private Image speedBar;
        [SerializeField] private int numPoints = 30;         // Number of points in the line
        [SerializeField] private float timeStep = 0.1f;     // Time interval between points

        private GameSettings Settings => GameSettingsProvider.Instance.GameSettings;

        private void OnEnable()
        {
            EventManager.updateBallInputEvent += OnBallInputUpdate;
            EventManager.throwBallEvent += HideTrajectory;
        }

        private void OnDisable()
        {
            EventManager.updateBallInputEvent -= OnBallInputUpdate;
            EventManager.throwBallEvent -= HideTrajectory;
        }

        private void Start()
        {
            trajectoryLine.positionCount = numPoints;
            trajectoryLine.enabled = false; // Hide initially
        }

        // Call this every frame while aiming / moving mouse
        public void OnBallInputUpdate((float speed, float angle, Vector2 direction) speedAngle)
        {
            trajectoryLine.enabled = true;

            // Calculate initial velocity
            float angleDegrees = Mathf.Lerp(Settings.minThrowAngle, Settings.maxThrowAngle, speedAngle.angle);
            float angleRad = angleDegrees * Mathf.Deg2Rad;

            Vector3 horizontalDir = new Vector3(speedAngle.direction.x, 0f, speedAngle.direction.y).normalized;
            Vector3 horizontalVelocity = horizontalDir * speedAngle.speed * Settings.maxThrowForceHorizontal;
            float verticalVelocity = Mathf.Sin(angleRad) * Settings.maxThrowForceVertical;

            Vector3 initialVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

            Vector3 startPos = transform.position;
            Vector3 gravity = Physics.gravity;

            // Fill LineRenderer positions
            for (int i = 0; i < numPoints; i++)
            {
                float t = i * timeStep;
                Vector3 displacement = initialVelocity * t + 0.5f * gravity * t * t;
                trajectoryLine.SetPosition(i, startPos + displacement);
            }

            speedBar.fillAmount = speedAngle.speed;
        }

        // Call this after the ball is thrown to hide the trajectory
        public void HideTrajectory((float speed, float angle, Vector2 direction) speedAngle)
        {
            trajectoryLine.enabled = false;
        }
    }
}