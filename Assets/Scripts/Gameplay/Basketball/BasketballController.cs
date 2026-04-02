using UnityEngine;

namespace Basketball_Demo.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasketballController : MonoBehaviour
    {
        [SerializeField] private float maxThrowForceHorizontal;
        [SerializeField] private float maxThrowForceVertical;
        [SerializeField] private float maxThrowAngle = 90;
        [SerializeField] private float minThrowAngle = 15;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        //private void OnThrowBall((float speed, float angle, Vector2 direction) speedAngle)
        //{
        //    rb.isKinematic = false;

        //    float angleDegrees = Mathf.LerpAngle(minThrowAngle, maxThrowAngle, speedAngle.angle);
        //    float angleRad = angleDegrees * Mathf.Deg2Rad;

        //    Vector3 horizontalDir = new Vector3(speedAngle.direction.x, 0f, speedAngle.direction.y).normalized;
        //    float horizontalStrength = Mathf.Cos(angleRad);
        //    float verticalStrength = Mathf.Sin(angleRad);
        //    Vector3 finalDirection = horizontalDir * horizontalStrength + Vector3.up * verticalStrength;

        //    rb.linearVelocity = finalDirection * speedAngle.speed * maxThrowForce;

        //    EventManager.throwBallEvent -= OnThrowBall;
        //}

        private void OnThrowBall((float speed, float angle, Vector2 direction) speedAngle)
        {
            rb.isKinematic = false;

            // 1. Convert angle
            float angleDegrees = Mathf.Lerp(minThrowAngle, maxThrowAngle, speedAngle.angle);
            float angleRad = angleDegrees * Mathf.Deg2Rad;

            // 2. Horizontal direction (XZ only)
            Vector3 horizontalDir = new Vector3(speedAngle.direction.x, 0f, speedAngle.direction.y).normalized;

            // 3. Horizontal velocity (ONLY speed affects this)
            Vector3 horizontalVelocity = horizontalDir * speedAngle.speed * maxThrowForceHorizontal;

            // 4. Vertical velocity (ONLY angle affects this)
            float verticalVelocity = Mathf.Sin(angleRad) * maxThrowForceVertical;

            // 5. Combine
            Vector3 finalVelocity = new Vector3(
                horizontalVelocity.x,
                verticalVelocity,
                horizontalVelocity.z
            );

            rb.linearVelocity = finalVelocity;

            EventManager.throwBallEvent -= OnThrowBall;
        }

        public void InitializeBasketball()
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            transform.localPosition = Vector3.zero;
            rb.isKinematic = true;

            EventManager.throwBallEvent += OnThrowBall;
        }
    }
}
