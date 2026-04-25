using System.Collections.Generic;
using UI;
using UnityEngine;
using UI.Scene;
using UI.Popup;
using UnityEngine.UI;
using Utils;

namespace Manager.Core
{
    public class UIManager
    {
        private int _order = 10;
        private UISceneBase _uiScene = null;
        private Stack<UIPopupBase> _uiPopupStack = new Stack<UIPopupBase>();
        private GameObject _uiRoot;
        public bool IsPopupUIOn { get { return _uiPopupStack.IsEmpty() == false; } }
        public void Init()
        {
            if (_uiRoot == null)
            {
                _uiRoot = GameObject.Find("@UI_Root");
                if (_uiRoot == null)
                {
                    _uiRoot = new GameObject { name = "@UI_Root" };
                }
            }
        }

        public void SetCanvas(GameObject go, Vector2 referenceResolution, bool sort = true, int sortOrder = 0)
        {
            Canvas canvas = go.GetOrAddComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.overrideSorting = true;
            }

            CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
            if (cs != null)
            {
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.referenceResolution = referenceResolution;//new Vector2(1920, 1080)
            }

            go.GetOrAddComponent<GraphicRaycaster>();

            if (sort)
            {
                canvas.sortingOrder = _order;
                _order++;
            }
            else
            {
                canvas.sortingOrder = sortOrder;
            }
        }

        public T MakeWorldSpaceUI<T>(string name = null, Transform parent = null, bool worldPositionStays = false) where T : UIBase
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(name,parent,worldPositionStays);
            go.transform.SetParent(parent, worldPositionStays);

            Canvas canvas = go.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            return go.GetOrAddComponent<T>();
        }

        public T MakeSubItem<T>(string name = null, Transform parent = null, bool worldPositionStays = false, bool pooling = true) where T : UIBase
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(name, parent,worldPositionStays ,pooling);
            go.transform.SetParent(parent, worldPositionStays);

            return go.GetOrAddComponent<T>();
        }

        public T ShowSceneUI<T>(string name = null, bool worldPositionStays = false) where T : UISceneBase
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(name,_uiRoot.transform,worldPositionStays);
            T sceneUI = go.GetOrAddComponent<T>();
            _uiScene = sceneUI;

            go.transform.SetParent(_uiRoot.transform, worldPositionStays);

            return sceneUI;
        }

        public T ShowPopupUI<T>(string name = null, Transform parent = null, bool worldPositionStays = false) where T : UIPopupBase
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(name,_uiRoot.transform, worldPositionStays);
            T popup = go.GetOrAddComponent<T>();
            _uiPopupStack.Push(popup);

            if(parent == null)
            {
                parent = _uiRoot.transform;
            }

            go.transform.SetParent(parent,worldPositionStays);

            return popup;
        }

        public void ClosePopupUI()
        {
            if (IsPopupUIOn)
            {
                return;
            }

            UIPopupBase popup = _uiPopupStack.Pop();

            Managers.Instance.ResourceManager.Destroy(popup.gameObject);
            popup = null;
            _order--;

        }

        public void CloseAllPopupUI()
        {
            while (IsPopupUIOn)//_uiPopupStack.Count > 0
            {
                ClosePopupUI();
            }
        }

        public int GetPopupCount()
        {
            return _uiPopupStack.Count;
        }

        public void Clear()
        {
            CloseAllPopupUI();
            _uiScene = null;
        }
    }
}