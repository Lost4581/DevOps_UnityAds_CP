using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsService : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsService Instance { get; private set; }

    [Header("Android IDs")]
    [SerializeField] private string androidGameId;
    [SerializeField] private string androidBannerId;
    [SerializeField] private string androidRewardedId;

    [Header("iOS IDs")]
    [SerializeField] private string iosGameId;
    [SerializeField] private string iosBannerId;
    [SerializeField] private string iosRewardedId;

    [Header("Settings")]
    [SerializeField] private bool testMode = true;
    [SerializeField] private bool showBannerOnStart = false;

    private string _gameId;
    private string _bannerId;
    private string _rewardedId;

    private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    public bool IsInitialized { get; private set; }
    public bool IsRewardedLoaded { get; private set; }

    public event Action OnRewardedCompleted;
    public event Action OnRewardedFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupPlatform();
    }

    private void SetupPlatform()
    {
        #if UNITY_ANDROID
            _gameId = androidGameId;
            _bannerId = androidBannerId;
            _rewardedId = androidRewardedId;
        #elif UNITY_IOS
            _gameId = iosGameId;
            _bannerId = iosBannerId;
            _rewardedId = iosRewardedId;
        #else
            _gameId = androidGameId;
            _bannerId = androidBannerId;
            _rewardedId = androidRewardedId;
            Debug.Log("[AdsService] Editor mode — используются Android IDs для тестирования");
        #endif

        Debug.Log($"[AdsService] Platform: {Application.platform}, GameId: {_gameId}");
    }

    private void Start()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        if (IsInitialized) return;

        Advertisement.Initialize(_gameId, testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("[AdsService] Unity Ads инициализирован!");
        IsInitialized = true;

        LoadRewarded();

        if (showBannerOnStart)
        {
            ShowBanner();
        }
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"[AdsService] Ошибка инициализации: {error} - {message}");
    }

    public void ShowBanner()
    {
        if (!IsInitialized) return;

        Advertisement.Banner.SetPosition(_bannerPosition);
        Advertisement.Banner.Load(_bannerId, new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        });
    }

    private void OnBannerLoaded()
    {
        Debug.Log("[AdsService] Баннер загружен");
        Advertisement.Banner.Show(_bannerId);
    }

    private void OnBannerError(string message)
    {
        Debug.LogError($"[AdsService] Ошибка баннера: {message}");
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    public void LoadRewarded()
    {
        if (!IsInitialized) return;

        Advertisement.Load(_rewardedId, this);
    }

    public void ShowRewarded()
    {
        if (!IsInitialized)
        {
            Debug.LogWarning("[AdsService] Реклама не инициализирована!");
            OnRewardedFailed?.Invoke();
            return;
        }

        if (!IsRewardedLoaded)
        {
            Debug.LogWarning("[AdsService] Rewarded не загружена, пробуем загрузить...");
            LoadRewarded();
            OnRewardedFailed?.Invoke();
            return;
        }

        Advertisement.Show(_rewardedId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"[AdsService] Загружено: {placementId}");
        if (placementId == _rewardedId)
        {
            IsRewardedLoaded = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"[AdsService] Ошибка загрузки {placementId}: {error} - {message}");
        if (placementId == _rewardedId)
        {
            IsRewardedLoaded = false;
        }
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == _rewardedId)
        {
            IsRewardedLoaded = false;

            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("[AdsService] Реклама досмотрена! Награда выдана.");
                OnRewardedCompleted?.Invoke();
            }
            else
            {
                Debug.Log("[AdsService] Реклама пропущена/не завершена.");
                OnRewardedFailed?.Invoke();
            }

            LoadRewarded();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"[AdsService] Ошибка показа {placementId}: {error} - {message}");
        if (placementId == _rewardedId)
        {
            IsRewardedLoaded = false;
            OnRewardedFailed?.Invoke();
            LoadRewarded();
        }
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"[AdsService] Показ начался: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"[AdsService] Клик по рекламе: {placementId}");
    }
}