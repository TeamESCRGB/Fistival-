using UnityEngine;
using DG.Tweening;
using Utils;
using UnityEngine.EventSystems;
using UI.Transition;

public class CoreTwoUIBookFlip : MonoBehaviour
{
    public GameObject tar;
    public bool a;
    public float v;
    [ContextMenu("a")]
    public void FlipToNextPage()
    {
        if(a)
        {
            tar.GetOrAddComponent<BookFlip>().Open(v);
        }
        else
        {
            tar.GetOrAddComponent<BookFlip>().Close(v);
        }
    }
}
