using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Scripts.TestScripts
{
    public class TestJump : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float force;
        public bool init=false;

        public float initialY;
        public float maxYDel=1;

        private void FixedUpdate()
        {
            if(init==false)
            {
                return;
            }
            if(transform.position.y >= initialY + maxYDel)
            {
                rb.gravityScale = 0;
                init = false;
                rb.linearVelocityY = 0;
                StartCoroutine(down());
            }
            
        }

        IEnumerator down()
        {
            yield return new WaitForSeconds(1);
            //rb.gravityScale = 1;
        }

        IEnumerator star()
        {
            yield return new WaitForSeconds(2);
            initialY = transform.position.y;
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            init= true;
        }

        [ContextMenu("jump")]
        void fun()
        {
            StartCoroutine(star());
        }

    }
}
