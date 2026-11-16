using UnityEngine;
using UnityEngine.UI;

public class AdsButtonHandler : MonoBehaviour
{
    [Header("Кнопки")]
    [SerializeField] private Button showBannerButton;
    [SerializeField] private Button hideBannerButton;
    [SerializeField] private Button showRewardedButton;

    [Header("Награда")]
    [SerializeField] private int rewardAmount = 100;

    private void Start()
    {
        if (showBannerButton != null)
            showBannerButton.onClick.AddListener(OnShowBannerClick);

        if (hideBannerButton != null)
            hideBannerButton.onClick.AddListener(OnHideBannerClick);

        if (showRewardedButton != null)
            showRewardedButton.onClick.AddListener(OnShowRewardedClick);

        if (AdsService.Instance != null)
        {
            AdsService.Instance.OnRewardedCompleted += HandleReward;
            AdsService.Instance.OnRewardedFailed += HandleRewardFailed;
        }
    }

    private void OnDestroy()
    {
        if (AdsService.Instance != null)
        {
            AdsService.Instance.OnRewardedCompleted -= HandleReward;
            AdsService.Instance.OnRewardedFailed -= HandleRewardFailed;
        }
    }

    private void OnShowBannerClick()
    {
        Debug.Log("[UI] Показать баннер");
        AdsService.Instance?.ShowBanner();
    }

    private void OnHideBannerClick()
    {
        Debug.Log("[UI] Скрыть баннер");
        AdsService.Instance?.HideBanner();
    }

    private void OnShowRewardedClick()
    {
        Debug.Log("[UI] Показать rewarded");
        AdsService.Instance?.ShowRewarded();
    }

    private void HandleReward()
    {
        Debug.Log($"[Reward] Получено {rewardAmount} монет!");
    }

    private void HandleRewardFailed()
    {
        Debug.Log("[Reward] Награда не выдана");
    }
}