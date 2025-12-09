using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InternetChecker : MonoBehaviour
{
    public static InternetChecker Instance;

    public GameObject noInternetPanel; // Assign in the inspector
    public Button retryButton;         // Assign in the inspector

    private bool isInternetAvailable = true;

    void Awake()
    {
        // Singleton logic to prevent duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (retryButton != null)
            retryButton.onClick.AddListener(CheckInternetManually);

        StartCoroutine(CheckInternetContinuously());
    }

    IEnumerator CheckInternetContinuously()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (!isInternetAvailable)
                {
                    RestoreGame();
                }
                isInternetAvailable = true;
            }
            else
            {
                if (isInternetAvailable)
                {
                    ShowNoInternetPanel();
                }
                isInternetAvailable = false;
            }
        }
    }

    void ShowNoInternetPanel()
    {
        if (noInternetPanel != null)
            noInternetPanel.SetActive(true);

        Time.timeScale = 0f; // Pause the game
    }

    void RestoreGame()
    {
        if (noInternetPanel != null)
            noInternetPanel.SetActive(false);

        Time.timeScale = 1f; // Resume the game
    }

    public void CheckInternetManually()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            RestoreGame();
            isInternetAvailable = true;
        }
        else
        {
            isInternetAvailable = false;
        }
    }
}
