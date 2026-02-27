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
        public bool IsPopupUIOn { get; private set; } = false;
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

        public void SetCanvas(GameObject go, bool sort = true, int sortOrder = 0)
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
                cs.referenceResolution = new Vector2(1080, 1920);
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

        public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UIBase
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(name);
            if (parent != null)
            {
                go.transform.SetParent(parent);
            }

            Canvas canvas = go.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            return go.GetOrAddComponent<T>();
        }

        public T MakeSubItem<T>(Transform parent = null, string name = null, bool pooling = true) where T : UIBase
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(name, parent, pooling);
            go.transform.SetParent(parent);
            return go.GetOrAddComponent<T>();
        }

        public T ShowSceneUI<T>(string name = null) where T : UISceneBase
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(name);
            T sceneUI = go.GetOrAddComponent<T>();
            _uiScene = sceneUI;

            go.transform.SetParent(_uiRoot.transform);

            return sceneUI;
        }

        public T ShowPopupUI<T>(string name = null) where T : UIPopupBase
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(name);
            T popup = go.GetOrAddComponent<T>();
            _uiPopupStack.Push(popup);

            go.transform.SetParent(_uiRoot.transform);
            IsPopupUIOn = true;

            return popup;
        }

        public void ClosePopupUI()
        {
            if (_uiPopupStack.Count <= 0)
            {
                return;
            }

            UIPopupBase popup = _uiPopupStack.Pop();

            Managers.Instance.ResourceManager.Destroy(popup.gameObject);
            popup = null;
            _order--;

            if(_uiPopupStack.Count <= 0)
            {
                IsPopupUIOn = false;
            }
        }

        public void CloseAllPopupUI()
        {
            while (_uiPopupStack.Count > 0)
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