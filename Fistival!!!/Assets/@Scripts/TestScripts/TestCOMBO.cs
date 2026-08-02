using Coordinator.Hands;
using Defines;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Scripts.TestScripts
{
    public class TestCOMBO : MonoBehaviour
    {
        public WWEHandCoordinator hand;
        public WWESkillTypes type;

        private void Start()
        {
            hand.OnComboChanged += OnCombo;
        }

        public void OnCombo(WWESkillTypes ty)
        {
            if(ty == WWESkillTypes.ACTIVATION)
            {
                Debug.Log($"{type.ToString()} 실행");
                type = WWESkillTypes.NORMAL;
                return;
            }
            if(ty ==WWESkillTypes.NORMAL)
            {
                Debug.Log("콤보 시전 실패");
                type = ty;
                return;
            }

            if(ty != WWESkillTypes.ACTIVATION)
            {
                type = ty;
                Debug.Log($"{ty.ToString()} 콤보 시전");
            }
        }
    }
}
