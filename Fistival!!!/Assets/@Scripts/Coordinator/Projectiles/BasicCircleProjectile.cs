using Manager;

namespace Coordinator.Projectiles
{
    public class BasicCircleProjectile : ProjectileCoordinator
    {
        public override void Destruct()
        {
            //여기에 소멸 애니메이션 넣든지 하기
            Managers.Instance.ResourceManager.Destroy(gameObject, true);
        }
    }
}