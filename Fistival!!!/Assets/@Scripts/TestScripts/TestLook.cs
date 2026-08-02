using Actor;
using Coordinator;
using Data;
using UnityEngine;

public class TestLook : MonoBehaviour
{
    public float speed;
    public Vector2 dir;
    private void Start()
    {
        ProjectileCoordinator c = GetComponent<ProjectileCoordinator>();

        ProjectileData data = new ProjectileData();
        data.Speed = speed;
        data.Damage = 200;
        data.AttackRadius = 1.5f;
        data.Physics2DMaterialName = "asdf";
        data.Lifetime = 5;
        c.Init(1<<12,data);

        c.Launch(transform.position, dir);
    }
}
