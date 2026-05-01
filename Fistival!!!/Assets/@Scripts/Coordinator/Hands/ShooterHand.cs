using ComponentModule;
using Coordinator;
using Defines;
using Manager;
using UnityEngine;
using Utils;

namespace Coordinator.Hands
{
    public class ShooterHand : HandCoordinatorBase
    {

        protected bool _isLMBPressed;
        protected Transform _attackBox;
        protected int _projectileIdx;
        protected CooldownComponentModule _cooldownModule;

        public void Init(int projectileIdx,Rigidbody2D parentrb2d , LayerMask attackableMask, float cooldownTime)
        {
            InitRMBOperations(parentrb2d,attackableMask);
            ResetEvents();
            _isLMBPressed = false;
            _projectileIdx = projectileIdx;
            _cooldownModule = Managers.Instance.CooldownManager.GetCooldownModule(cooldownTime,-1);
        }

        private void OnDisable()
        {
            if( _cooldownModule != null )
            {
                Managers.Instance.CooldownManager.ReturnModule(_cooldownModule);
                _cooldownModule = null;
                _isLMBPressed =false;
            }
        }

        protected override void OnAwake()
        {
            _attackBox = transform.Find("@AttackBox");
            base.OnAwake();

#if UNITY_EDITOR
            if (_attackBox == null)
            {
                Debug.LogError($"@AttackBox 가 {gameObject.name}의 자식중에 없습니다.");
            }
#endif
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if(_isLMBPressed && _cooldownModule.IsCooldownEnded())
            {
                _cooldownModule.StartCooldown();
                ProjectileLaunchHelper.LaunchConstantDir(_attackableMask, _projectileIdx, _attackBox.position, Vector2.right);
            }

        }

        public override void OnLMBPressed()
        {
            _isLMBPressed = true;
        }

        public override void OnLMBReleased()
        {
            _isLMBPressed = false;
        }
    }
}
