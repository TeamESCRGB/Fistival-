using Coordinator;
using UnityEngine;

public class PV_2D_SHOOT_STARTER : MonoBehaviour
{
    [SerializeField]
    PV2DShooterCoord co;

    private bool _isTriggered;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_isTriggered)
        {
            return;
        }
        _isTriggered = true;
        Camera.main.GetComponent<SmoothFollowCoordinator>().SetFollowState(false);
        var pos = transform.position;
        pos.z = -10;
        Camera.main.transform.position = pos;
        co.Init();
    }
}
