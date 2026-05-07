using Defines;
using UnityEngine;

namespace Utils
{
    public static class MovementUtils
    {
        private const float _addSpeedThreshold = 0.001f;
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
    }
}