#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.IO;
using System.Text;

namespace Editor
{
    public class NewInputSystemManagerAutoCreator : EditorWindow
    {
        private InputActionAsset targetAsset;
        private string savePath = "Assets/@Scripts/Managers/Core/NewInputSystemManager.cs";

        [MenuItem("Tools/Input System Binder Generator")]
        public static void ShowWindow()
        {
            GetWindow<NewInputSystemManagerAutoCreator>("Input Binder Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("인풋 매니저 템플릿 코드 생성기", EditorStyles.boldLabel);

            targetAsset = (InputActionAsset)EditorGUILayout.ObjectField("Input Action Asset", targetAsset, typeof(InputActionAsset), false);
            savePath = EditorGUILayout.TextField("Save Path", savePath);

            if (GUILayout.Button("매니저 스크립트 생성"))
            {
                if (targetAsset == null)
                {
                    EditorUtility.DisplayDialog("오류", "InputActionAsset을 지정해 주세요!", "확인");
                    return;
                }

                GenerateCode();
            }
        }

        private void GenerateCode()
        {
            StringBuilder publicEvents = new StringBuilder();
            StringBuilder eventClears = new StringBuilder();
            StringBuilder playerFunctions = new StringBuilder();
            StringBuilder uiFunctions = new StringBuilder();

            foreach (InputActionMap map in targetAsset.actionMaps)
            {
                // 원본 대소문자 유지 (공백만 제거)
                string cleanedMapName = map.name.Replace(" ", "");
                bool isPlayerMap = cleanedMapName.Equals("Player", System.StringComparison.OrdinalIgnoreCase);

                foreach (InputAction action in map.actions)
                {
                    // 원본 대소문자 유지 (공백만 제거)
                    string cleanedActionName = action.name.Replace(" ", "");

                    // [요청 반영] On 제거, ToUpper 제거 -> 에셋 이름 조합 형태
                    string eventName = $"{cleanedMapName}_{cleanedActionName}";
                    string inputFunctionName = $"On{cleanedMapName}_{cleanedActionName}";

                    // 1. Public 이벤트 생성
                    publicEvents.AppendLine($"        public event Action<InputAction.CallbackContext> {eventName};");

                    // 2. Clear() 초기화 생성
                    eventClears.AppendLine($"            {eventName} = null;");

                    // 3. PlayerInput 바인딩용 중계 함수 생성
                    StringBuilder funcSb = new StringBuilder();
                    funcSb.AppendLine($"        public void {inputFunctionName}(InputAction.CallbackContext ctx)");
                    funcSb.AppendLine("        {");
                    funcSb.AppendLine($"            {eventName}?.Invoke(ctx);");
                    funcSb.AppendLine("        }");
                    funcSb.AppendLine();

                    if (isPlayerMap)
                    {
                        playerFunctions.Append(funcSb.ToString());
                    }
                    else
                    {
                        uiFunctions.Append(funcSb.ToString());
                    }
                }
            }

            // 원본 템플릿 결합
            StringBuilder template = new StringBuilder();
            template.AppendLine("using Defines;");
            template.AppendLine("using System;");
            template.AppendLine("using UnityEngine;");
            template.AppendLine("using UnityEngine.InputSystem;");
            template.AppendLine();
            template.AppendLine("namespace Manager.Core");
            template.AppendLine("{");
            template.AppendLine("    public class NewInputSystemManager : MonoBehaviour");
            template.AppendLine("    {");
            template.AppendLine("        private PlayerInput _input;");
            template.AppendLine("        public Action<ActionMapTypes> OnActionMapChanged;");

            template.Append(publicEvents.ToString());
            template.AppendLine();

            template.AppendLine("        private void Awake()");
            template.AppendLine("        {");
            template.AppendLine("            _input = GetComponent<PlayerInput>();");
            template.AppendLine("#if UNITY_EDITOR");
            template.AppendLine("            Debug.Assert(_input != null, $\"{name}에 PlayerInput이 없습니다\");");
            template.AppendLine("#endif");
            template.AppendLine("        }");
            template.AppendLine();

            template.AppendLine("        public void SwitchActionMap(ActionMapTypes actionMap)");
            template.AppendLine("        {");
            template.AppendLine("            _input.SwitchCurrentActionMap(actionMap.ToString());");
            template.AppendLine("            OnActionMapChanged?.Invoke(actionMap);");
            template.AppendLine("        }");
            template.AppendLine();

            template.AppendLine("        public void Clear()");
            template.AppendLine("        {");
            template.AppendLine("            OnActionMapChanged = null;");

            template.Append(eventClears.ToString());
            template.AppendLine("        }");
            template.AppendLine();

            template.AppendLine("        #region Player");
            template.Append(playerFunctions.ToString());
            template.AppendLine("        #endregion");
            template.AppendLine();

            template.AppendLine("        #region UI");
            template.Append(uiFunctions.ToString());
            template.AppendLine("        #endregion");

            template.AppendLine("    }");
            template.AppendLine("}");

            File.WriteAllText(savePath, template.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("성공", "가독성이 개선된 인풋 매니저 코드가 생성되었습니다!", "확인");
        }
    }
}
#endif