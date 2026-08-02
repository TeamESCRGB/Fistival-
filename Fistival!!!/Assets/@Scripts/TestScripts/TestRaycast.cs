using UnityEngine;
using static Utils.VectorUtils;
public class TestRaycast : MonoBehaviour
{
    public Transform box;
    public Collider2D col;

    public RaycastHit2D hit;

    public Vector2 dir;
    public float angle;

    public Transform target;

    private void Update()
    {
        if (box == null) return;

        dir = GetDirVec2(target.position, transform.position);
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        hit = Physics2D.BoxCast(transform.position, new Vector2(0.1f, box.localScale.y), angle, dir, box.localScale.x, 1<<1);
        col = hit.collider;
    }

    private void OnDrawGizmos()
    {
        // 1. 박스의 시작 지점 (현재 위치)
        Gizmos.color = Color.yellow;
        DrawGizmoBox(transform.position, new Vector2(0.1f, box.localScale.y), angle);

        if (hit.collider != null)
        {
            // 2. 충돌 시: 발사 경로를 선으로 표시
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, hit.centroid);

            // 3. 충돌 지점에서의 박스 모습
            DrawGizmoBox(hit.centroid, new Vector2(0.1f, box.localScale.y), angle);

            // 4. 충돌 지점(Point)과 법선(Normal) 표시
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(hit.point, 0.05f);
            Gizmos.DrawRay(hit.point, hit.normal * 0.5f);
        }
        else
        {
            // 충돌 안 했을 때: 최대 거리만큼 가상의 경로 표시
            Gizmos.color = Color.green;
            Vector2 endPos = (Vector2)transform.position + (dir.normalized * box.localScale.x);
            Gizmos.DrawLine(transform.position, endPos);
            DrawGizmoBox(endPos, new Vector2(0.1f, box.localScale.y), angle);
        }
    }

    // 회전된 박스를 그리기 위한 보조 메서드
    private void DrawGizmoBox(Vector2 center, Vector2 size, float angle)
    {
        Matrix4x4 savedMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = savedMatrix;
    }

}
