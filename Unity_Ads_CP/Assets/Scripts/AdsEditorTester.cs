using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AdsService))]
public class AdsEditorTester : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AdsService ads = (AdsService)target;

        EditorGUILayout.Space(20);
        EditorGUILayout.HelpBox("Тестовые кнопки (только в редакторе)", MessageType.Info);

        GUI.enabled = ads.IsInitialized;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Показать баннер", GUILayout.Height(30)))
        {
            ads.ShowBanner();
        }
        if (GUILayout.Button("Скрыть баннер", GUILayout.Height(30)))
        {
            ads.HideBanner();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Загрузить Rewarded", GUILayout.Height(30)))
        {
            ads.LoadRewarded();
        }

        GUI.enabled = ads.IsInitialized && ads.IsRewardedLoaded;
        if (GUILayout.Button("Показать Rewarded", GUILayout.Height(35)))
        {
            ads.ShowRewarded();
        }

        GUI.enabled = true;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField($"Статус: {(ads.IsInitialized ? "Инициализирован" : "Ожидание")}");
        EditorGUILayout.LabelField($"Rewarded: {(ads.IsRewardedLoaded ? "Загружена" : "Не загружена")}");
    }
}
#endif