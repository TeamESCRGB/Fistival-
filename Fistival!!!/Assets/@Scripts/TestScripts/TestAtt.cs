using Coordinator.Movements;
using Coordinator.Skills;
using Coordinator.Victims;
using InputHandler;
using UnityEngine;

public class TestAtt : MonoBehaviour
{
    public TouchDamageSkill a;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        a.Init(layer, 1, 0.5f,5);
    }
    public LayerMask layer;
    // Update is called once per frame
    void Update()
    {
        var res = Physics2D.OverlapBox(transform.position, transform.localScale, 0, layer);
        if(res != null)
        {
            Debug.Log($"{res.name} {res.TryGetComponent<IAttackable>(out var _)}");
        }
    }
}
