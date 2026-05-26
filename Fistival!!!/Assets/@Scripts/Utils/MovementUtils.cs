using Defines;
using UnityEngine;

namespace Utils
{
    public static class MovementUtils
    {
        private const float _addSpeedThreshold = 0.001f;
        private const float _dampingThreshold = 0.001f;
        public static float CalculateNewSpeed(float nowSpeed, float maxSpeed, float movDir, ref MovementState state)
        {
            float deficitSpeed = CalculateDeficitSpeed(nowSpeed,maxSpeed, movDir);


            if(state == MovementState.START_REQ)
            {
                state = MovementState.START_ACCEPT;

                nowSpeed += maxSpeed * movDir;//이거 그대로 넣으면 초반에 속도 팍 튄다
                deficitSpeed = 0;
            }
            else if(state == MovementState.STOP_REQ)
            {
                bool prevDirNeg = nowSpeed < 0;
                state = MovementState.STOP_ACCEPT;
                nowSpeed -= movDir * maxSpeed;

                if(nowSpeed < 0)
                {
                    nowSpeed += maxSpeed;
                }
                else
                {
                    nowSpeed -= maxSpeed;
                }

                if(prevDirNeg != nowSpeed< 0)
                {
                    nowSpeed = 0;
                }
            }

            return nowSpeed + deficitSpeed;
        }

        public static float CalculateDeficitSpeed(float nowSpeed, float maxSpeed, float movDir)
        {
            float speedFromMovdir = nowSpeed * movDir;
            float deficitSpeed = maxSpeed - speedFromMovdir;

            if(deficitSpeed <= _addSpeedThreshold)
            {
                return 0;
            }

            return  movDir * deficitSpeed;
        }

        public static Vector2 CaculateThrowPower(Vector2 distance, float totalMoveTime, float linearDamping, float dampingThreshold, float gravityConstant, in Vector2 currentVelocity)
        {
            Vector2 dis = distance;
            float t = totalMoveTime;

            float d = linearDamping;
            float dampingFactor = 1.0f;

            if (d > _dampingThreshold)
            {
                dampingFactor = (d * t) / (1.0f - Mathf.Exp(-d * t));
            }

            Vector2 targetSpd;
            targetSpd.x = dis.x / t;

            if (dis.y < 0)
            {
                targetSpd.y = (dis.y / t) - (0.5f * gravityConstant * t);
            }
            else
            {
                targetSpd.y = (dis.y / t) + (0.5f * gravityConstant * t);
            }

            targetSpd *= dampingFactor;

            Vector2 impulseForce = targetSpd - currentVelocity;

            return impulseForce;
        }
    }
}