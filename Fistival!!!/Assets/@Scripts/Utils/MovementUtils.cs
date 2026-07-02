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

        public static Vector2 PreciseCalculateThrowPower(Vector2 distance, float totalMoveTime, float linearDamping, float dampingThreshold, float gravityAbsolute, in Vector2 currentVelocity)
        {
            Vector2 dis = distance;
            float t = totalMoveTime;

            if (t <= 0.001f) return Vector2.zero;

            float d = linearDamping;
            Vector2 targetSpd = Vector2.zero;

            // 공기 저항(Damping)이 거의 없을 때 (기본 등가속도 공식)
            if (d <= dampingThreshold)
            {
                targetSpd.x = dis.x / t;
                targetSpd.y = (dis.y / t) + (0.5f * gravityAbsolute * t);
            }
            // 공기 저항이 존재할 때 (정확한 미분방정식 역산)
            else
            {
                // 공기 저항에 의해 감쇠되는 핵심 배수 (e^-dt)
                float expFactor = Mathf.Exp(-d * t);

                // 1. 가로(X) 축 초기 속도 역산
                targetSpd.x = (dis.x * d) / (1.0f - expFactor);

                // 2. 세로(Y) 축 초기 속도 역산 (중력은 저항과 분리하여 수학적으로 정확히 매칭)
                // 유니티 2D/3D 기본 중력은 음수(-) 방향이므로 아래 식 구조가 완벽히 대응됩니다.
                float numeratorY = dis.y * d + gravityAbsolute * t - (gravityAbsolute / d) * (1.0f - expFactor);
                targetSpd.y = numeratorY / (1.0f - expFactor);
            }

            // 3. 현재 속도를 빼주어 최종 충격량(Impulse) 산출
            Vector2 impulseForce = targetSpd - currentVelocity;
            return impulseForce;
        }

        public static Vector2 ApproximatedCalculateThrowPower(Vector2 distance, float totalMoveTime, float linearDamping, float dampingThreshold, float gravityConstant, in Vector2 currentVelocity)
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