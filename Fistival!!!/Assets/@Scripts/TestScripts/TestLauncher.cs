using Manager;
using System.Collections;
using UnityEngine;
using Utils;

public class TestLauncher : MonoBehaviour
{
    public int idx;
    public LayerMask mask;
    [ContextMenu("laun")]
    public void fun()
    {
        var data = Managers.Instance.DataManager.ProjectileDataDict[idx];
        ProjectileLaunchHelper.LaunchConstantDir(mask, idx, transform.position, Vector2.right);
    }

    public Transform p;

    [ContextMenu("laun2")]
    public void ㄴㅁㅇㄻㄴㄹ()
    {
        StartCoroutine(asdf());
    }

    IEnumerator asdf()
    {
        yield return new WaitForSeconds(0.5f);
        var data = Managers.Instance.DataManager.ProjectileDataDict[idx];
        ProjectileLaunchHelper.LaunchConstantDir(mask,idx,transform.position,Vector2.left);
        //ProjectileLaunchHelper.LaunchGravityProjectile(mask, idx, transform.position, p);
    }
}
