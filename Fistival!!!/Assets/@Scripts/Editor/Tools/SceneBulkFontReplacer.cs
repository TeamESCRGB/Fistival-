#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement; // 씬 저장을 위해 필요
using TMPro;

public class SceneBulkFontReplacer : EditorWindow
{
    private TMP_FontAsset targetFont;

    [MenuItem("Tools/Scene Font Replacer")]
    public static void ShowWindow()
    {
        GetWindow<SceneBulkFontReplacer>("씬 폰트 교체기");
    }

    private void OnGUI()
    {
        GUILayout.Label("현재 열린 씬의 모든 폰트 변경", EditorStyles.boldLabel);
        targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField("새로운 TMP 폰트", targetFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("씬 전체 폰트 일괄 교체 및 저장"))
        {
            if (targetFont == null)
            {
                Debug.LogError("적용할 폰트 에셋을 할당해주세요.");
                return;
            }

            // 씬에 존재하는 오브젝트 중 TextMeshProUGUI를 사용하는 모든 컴포넌트 탐색
            // (Resources.FindObjectsOfTypeAll을 쓰면 비활성화된 하이어라키 오브젝트까지 완벽하게 찾습니다)
            TextMeshProUGUI[] allTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
            int count = 0;

            foreach (var text in allTexts)
            {
                // 프리펩 에셋 자체가 아닌, 실제 씬(Hierarchy)에 존재하는 오브젝트인지 검증
                if (!EditorUtility.IsPersistent(text.transform.root.gameObject) && text.hideFlags == HideFlags.None)
                {
                    // 변경 전 데이터 기록 (실수했을 때 Ctrl + Z 되돌리기가 가능하도록 지원)
                    Undo.RecordObject(text, "Change TMP Font");

                    text.font = targetFont;

                    // 에디터에게 해당 오브젝트의 데이터가 변경되었음을 인지시킴 (저장 대상 등록)
                    EditorUtility.SetDirty(text);
                    count++;
                }
            }

            // 변경된 내용이 있는 경우 씬 파일을 강제로 저장 상태로 만듦
            if (count > 0)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                Debug.Log($"성공: 현재 씬에서 총 {count}개의 TMP 폰트를 영구 변경했습니다.");
            }
        }
    }
}
#endif
