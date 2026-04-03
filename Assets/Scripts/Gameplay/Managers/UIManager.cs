using Basketball_Demo.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Basketball_Demo.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject lifeImgPrefab;
        [SerializeField] private GameObject gameoverPanel;
        [SerializeField] private GameObject helpPanel;
        [SerializeField] private Transform livesParent;
        [SerializeField] private TMP_Text pointsTMP;
        [SerializeField] private TMP_Text pointsGameOverTMP;
        [SerializeField] private Button restartButton;

        private void OnEnable()
        {
            EventManager.gameStateUpdateEvent += OnGameStateUpdated;
            EventManager.gameEndEvent += EndGame;

            restartButton.onClick.AddListener(RestartGame);
        }

        private void OnDisable()
        {
            EventManager.gameStateUpdateEvent -= OnGameStateUpdated;
            EventManager.gameEndEvent -= EndGame;

            restartButton.onClick.RemoveListener(RestartGame);
        }

        private void Start()
        {
            UpdateLives(GameSettingsProvider.Instance.GameSettings.maxMissesAllowed);
            gameoverPanel.SetActive(false);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.H) && !helpPanel.activeSelf)
            {
                helpPanel.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.H) && helpPanel.activeSelf)
            {
                helpPanel.SetActive(false);
            }
        }

        private void OnGameStateUpdated()
        {
            UpdateLives(GameSettingsProvider.Instance.GameSettings.maxMissesAllowed - GameFlow.Instance.GameState.ShotsMissed);
            pointsTMP.text = string.Format($"Points: {GameFlow.Instance.GameState.GameScore}", "00");
        }

        private void UpdateLives(int totalLivesLeft)
        {
            int count = livesParent.childCount;
            int difference = totalLivesLeft - count;

            if (difference > 0)
            {
                for(int i = 0; i < difference; i++)
                {
                    Instantiate(lifeImgPrefab, livesParent);
                }
            }

            if(difference < 0)
            {
                for (int i = 0; i > difference; i--)
                {
                    Destroy(livesParent.GetChild(0).gameObject);
                }
            }
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void EndGame()
        {
            gameoverPanel?.SetActive(true);
            pointsGameOverTMP.text = string.Format($"Points: {GameFlow.Instance.GameState.GameScore}", "00");
        }
    }
}
