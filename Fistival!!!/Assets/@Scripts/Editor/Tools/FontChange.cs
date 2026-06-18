#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;

public class FontChange : EditorWindow
{
    private TMP_FontAsset targetFont;

    [MenuItem("Tools/Bulk Font Changer")]
    public static void ShowWindow()
    {
        GetWindow<FontChange>("Font Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("프리펩 폰트 일괄 변경 툴", EditorStyles.boldLabel);
        targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField("새로운 폰트", targetFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("선택한 프리펩 파일들 폰트 바꾸기"))
        {
            if (targetFont == null)
            {
                Debug.LogError("변경할 폰트 에셋을 지정해주세요.");
                return;
            }

            // 프로젝트 창에서 선택한 에셋들 중 프리펩만 골라냄
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedObject);

                // 프리펩 파일 로드
                GameObject prefab = PrefabUtility.LoadPrefabContents(assetPath);
                TextMeshProUGUI[] tmps = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);

                if (tmps.Length > 0)
                {
                    foreach (var tmp in tmps)
                    {
                        tmp.font = targetFont;
                    }

                    // 프리펩 변경 사항 저장 및 연결 해제
                    PrefabUtility.SaveAsPrefabAsset(prefab, assetPath);
                    Debug.Log($"{selectedObject.name} 프리펩의 폰트를 변경했습니다.");
                }

                PrefabUtility.UnloadPrefabContents(prefab);
            }
            AssetDatabase.SaveAssets();
        }
    }
}
#endif