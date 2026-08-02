using DG.Tweening;
using UnityEngine;

public class testtween : MonoBehaviour
{
    public float _spawnMovementDelta;
    public float _spawnMovementDuration;
    [ContextMenu("asdf")]
    public void StartPlatformerPhase2()
    {
        transform.DOMoveY(_spawnMovementDelta, _spawnMovementDuration).SetRelative(true).From(-_spawnMovementDelta).onComplete += () => {Debug.Log("a"); };
    }
}
