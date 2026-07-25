using Coordinator.Objects;
using Data;
using Manager;
using System;
using UnityEngine;

namespace Coordinator.MobActs
{
    public class SlamAct : MobActBase
    {
        private ObjectData _objData;
        private int _objCnt;
        private Transform _player;
        private Vector2 _jumpForce;
        private Rigidbody2D _rb2d;
        private float _objDropForce;
        private bool _canSpawnObj;
        private LayerMask _ground;
        private LayerMask _attackLayer;
        private BoxCollider2D _hitbox;

        [SerializeField]
        private string _slamJumpSFX = "FastJumpUpSFX";
        [SerializeField]
        private string _slamEndSFX = "SlamEndSFX";

        private void Awake()
        {
            _hitbox = transform.Find("@Hitbox").GetComponent<BoxCollider2D>();
        }

        public void Init(Action onActionEnd, Animator animator, Rigidbody2D rb2d , int objIdx, int objCnt, Transform player, float jumpForce, float objDropForce, LayerMask groundLayer, LayerMask attackLayer)
        {
            base.Init(onActionEnd, animator);
            _canSpawnObj = false;
            _ground = groundLayer;
            _attackLayer= attackLayer;
            Managers.Instance.DataManager.ObjectDataDict.TryGetValue(objIdx, out _objData);
            _rb2d = rb2d;
            _objCnt= objCnt;
            _player = player;
            _objDropForce = objDropForce;
            _jumpForce = new Vector2(0, jumpForce);
            _hitbox.excludeLayers &= ~_attackLayer;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(_canSpawnObj == false)
            {
                return;
            }
            _hitbox.excludeLayers &= ~_attackLayer;
            if (_objData is null || ((1<<collision.collider.gameObject.layer)&_ground)==0)
            {
                _canSpawnObj = false;
                return;
            }
            var pos = transform.position;
            pos.y -= transform.lossyScale.y / 2;
            for(int i = 0; i < _objCnt; i++)
            {
                float sign = (i & 1) == 1 ? 1 : -1;
                var force = new Vector2(sign*_objDropForce,_objDropForce);
                var obj = Managers.Instance.ResourceManager.Instantiate(_objData.PrefabKey, null, true, true).GetComponentInChildren<ObjectCoordinator>();
                obj.transform.position = pos;
                obj.Init(_objData);
                obj.GetComponent<Rigidbody2D>().AddForce(force,ForceMode2D.Impulse);
            }
            Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, _slamEndSFX, false, Managers.Instance.GameManager.SFXVolume);
            _canSpawnObj = false;
        }
        //땅에 닿기 전까지 오브젝트,공격 레이어하고 충돌 안하게 하기
        public void SlamEnd()
        {
            _onActEnd?.Invoke();
        }

        public void Slam()
        {
            _rb2d.simulated = true;
            _rb2d.WakeUp();
            _rb2d.AddForce(-_jumpForce, ForceMode2D.Impulse);
            _canSpawnObj = true;
        }

        public void SlamStay()
        {
            var pos = _rb2d.transform.position;
            pos.x = _player.position.x;
            _rb2d.transform.position = pos;
            _rb2d.linearVelocity = Vector2.zero;
            _rb2d.simulated = false;
        }

        public void JumpForSlam()
        {
            _rb2d.AddForce(_jumpForce,ForceMode2D.Impulse);
            Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, _slamJumpSFX, false, Managers.Instance.GameManager.SFXVolume);
        }

        public override void Act()
        {
            _canSpawnObj = false;
            _hitbox.excludeLayers |= _attackLayer;
            _animator.SetTrigger("Slam");
        }

        public override void StopAct()
        {
            //unused
        }

        //행동 과정:
        //준비->점프->알림 띄워주기-3초 후->알림 사라지기->내려찍기->오브젝트 생성
    }
}
