using UnityEngine;

namespace Basketball_Demo
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private AudioSource audioSource;

        [Header("Audio Files")]
        public AudioClip throwBall;
        public AudioClip scoreBasket;
        public AudioClip ballHitRing;
        public AudioClip ballHitGround;
        public AudioClip newRound;

        public void PlayOneShot(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
