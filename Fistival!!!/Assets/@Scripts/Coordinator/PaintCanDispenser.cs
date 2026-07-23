using Coordinator;
using Coordinator.Objects;
using Data;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Scripts.Coordinator
{
    public class PaintCanDispenser : MonoBehaviour, IBasicInitializer
    {
        [SerializeField]
        private List<ButtonCoordinator> _buttons;
        private int _activateCnt = 0;
        [SerializeField]
        private int _objectIdx;
        private ObjectData _obj;
        private WaitForSeconds _waiter;
        [SerializeField]
        private float _waitTime;

        private Coroutine _resetRoutine;
        private bool _isResetting;
        private void Start()
        {
            _waiter = new WaitForSeconds(_waitTime);
            _obj = Managers.Instance.DataManager.ObjectDataDict[_objectIdx];
        }

        public void Init()
        {
            if(_resetRoutine != null && _isResetting)
            {
                StopCoroutine(_resetRoutine);
            }
            _isResetting = false;
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Init(false);
                _buttons[i].RegisterOnStateChanged(OnButtonStateChanged);
            }
            _activateCnt = 0;
        }

        private void OnButtonStateChanged(bool state)
        {
            if(state)
            {
                _activateCnt++;
            }

            if(_activateCnt >= _buttons.Count)
            {
                _activateCnt = 0;
                var go = Managers.Instance.ResourceManager.Instantiate(_obj.PrefabKey);
                if(go.TryGetComponent<ObjectCoordinator>(out var comp))
                {
                    comp.Init(_obj);
                    go.transform.position = transform.position;
                }
                else
                {
                    Managers.Instance.ResourceManager.Destroy(go);
                }
                _resetRoutine=StartCoroutine(ResetButtons());
                _isResetting = true;
            }
        }

        private IEnumerator ResetButtons()
        {
            yield return _waiter;
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].SetState(false);
            }
            _activateCnt = 0;
        }
    }
}
