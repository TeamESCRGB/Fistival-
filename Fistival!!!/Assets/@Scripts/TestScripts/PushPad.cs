using Coordinator.Hands;
using Coordinator.Movements;
using Coordinator.Victims;
using System.Collections;
using UnityEngine;

public class PushPad : MonoBehaviour
{
    public Vector2 force;
    public bool asdf;
    public IMovementLockable comm;
    public float cool;

    public RootShooterHand rhandTest;

    [ContextMenu("reload")]
    public void Call()
    {
        rhandTest?.Reload();
    }

    [ContextMenu("asdfasfd")]
    void ffasdfasf()
    {

    }

    IEnumerator llll()
    {
        asdf = false;
        yield return new WaitForSeconds(cool);
        asdf = true;
        yield return new WaitForSeconds(1);
        comm?.UnlockMovement();
    }



    private void FixedUpdate()
    {



        if(asdf)
        {
            var col = Physics2D.OverlapBox(transform.position, transform.localScale, 0);

            var com = col?.GetComponentInChildren<IPushable>();
            var test = col?.GetComponentInChildren<PlayerVictimCoordinator>();
            test?.StunFor(cool);

            com?.PushTo(force);
            //if(col != null && com != null)
            //{;
            //    comm = col?.GetComponentInChildren<IMovementLockable>();

            //    comm?.LockMovement();
            //    StartCoroutine(llll());
            //}
        }
    }
}
