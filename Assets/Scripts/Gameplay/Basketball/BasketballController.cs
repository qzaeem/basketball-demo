using UnityEngine;

namespace Basketball_Demo.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasketballController : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private const string topTriggerTag = "Trigger Top";
        private const string bottomTriggerTag = "Trigger Bottom";
        private const string groundTag = "Ground";
        private const string ringTag = "Ring";

        private GameSettings Settings => GameSettingsProvider.Instance.GameSettings;

        private Rigidbody rb;
        private bool topTriggerHit;
        private bool checkCollision;
        private bool shakeCamera;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnThrowBall((float speed, float angle, Vector2 direction) speedAngle)
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.throwBall);

            rb.isKinematic = false;

            float angleDegrees = Mathf.Lerp(Settings.minThrowAngle, Settings.maxThrowAngle, speedAngle.angle);
            float angleRad = angleDegrees * Mathf.Deg2Rad;

            Vector3 horizontalDir = new Vector3(speedAngle.direction.x, 0f, speedAngle.direction.y).normalized;

            Vector3 horizontalVelocity = horizontalDir * speedAngle.speed * Settings.maxThrowForceHorizontal;

            float verticalVelocity = Mathf.Sin(angleRad) * Settings.maxThrowForceVertical;

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

            shakeCamera = true;
            topTriggerHit = false;
            checkCollision = true;

            EventManager.throwBallEvent += OnThrowBall;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag(groundTag))
            {
                audioSource.PlayOneShot(SoundManager.Instance.ballHitGround);
            }

            if (collision.collider.CompareTag(ringTag))
            {
                audioSource.PlayOneShot(SoundManager.Instance.ballHitRing);

                if (shakeCamera)
                {
                    shakeCamera = false;
                    ObjectShaker.Instance.StartShake(0.5f);
                }
            }

            if (!checkCollision || GameFlow.Instance.GameState.HasGameEnded)
            {
                return;
            }

            if (collision.collider.CompareTag(groundTag))
            {
                checkCollision = false;
                EventManager.InvokeMissedBasketEvent();
                return;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!checkCollision || GameFlow.Instance.GameState.HasGameEnded)
            {
                return;
            }

            if(other.CompareTag(topTriggerTag))
            {
                topTriggerHit = true;
                return;
            }
            
            if(other.CompareTag(bottomTriggerTag) 
                && topTriggerHit)
            {
                EventManager.InvokeMadeBasketEvent();
                topTriggerHit= false;
                checkCollision = false;
                shakeCamera = false;

                other.GetComponent<BasketAnimation>().PlayBasketAnimation();
            }
        }
    }
}
