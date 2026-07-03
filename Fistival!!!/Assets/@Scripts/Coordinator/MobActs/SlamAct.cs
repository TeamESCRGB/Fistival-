using System;
using UnityEngine;

namespace Coordinator.MobActs
{
    public class SlamAct : MobActBase
    {
        private int _itemIdx;
        private int _itemCnt;
        private Transform _player;
        private Vector2 _jumpForce;
        private Rigidbody2D _rb2d;

        public void Init(Action onActionEnd, Animator animator, Rigidbody2D rb2d ,int itemIdx, int itemCnt, Transform player, float jumpForce)
        {
            base.Init(onActionEnd, animator);
            _rb2d = rb2d;
            _itemIdx= itemIdx;
            _itemCnt= itemCnt;
            _player = player;
            _jumpForce = new Vector2(0, jumpForce);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
           
        }

        public void SlamEnd()
        {
            _onActEnd?.Invoke();
        }

        public void Slam()
        {
            _rb2d.simulated = true;
            _rb2d.WakeUp();
            _rb2d.AddForce(-_jumpForce, ForceMode2D.Impulse);
        }

        public void Stay()
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
        }

        public override void Act()
        {
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
