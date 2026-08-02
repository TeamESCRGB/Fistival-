using Coordinator.Rhythm;
using Defines;
using InputHandler;
using Manager;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestParryInput : MonoBehaviour, ILMBInputHandler, IParrableObject
{
    public void OnFunc(InputAction.CallbackContext cb)
    {
        if(cb.started)
        {
            return;
        }

        OnLMBEvent(cb.control.IsPressed(), Vector2.zero);
    }

    public int nowIdx;
    public int endIdx;
    public NoteTypes noteType;
    public JudgementTypes judgeType;

    public void OnLMBEvent(bool pressed, Vector2 screenPos)
    {
        
        if(pressed)
        {
            var res = Managers.Instance.RhythmModeManager.ClickParry(this);
            Debug.Log($"누름 {res.nowIdx} {res.noteType} {res.judgeType}");

            nowIdx = res.nowIdx;
            endIdx = res.endIdx;
            noteType = res.noteType;
            judgeType = res.judgeType;
        }
        else
        {
            var res = Managers.Instance.RhythmModeManager.ReleaseParry(endIdx,this);

            Debug.Log($"땜 {res.nowIdx} {res.judgeType} {res.noteType}");
        }

    }

    public bool IsParrySuccess(int idx, NoteTypes noteType)
    {
        throw new System.NotImplementedException();
    }

    public void AddParryDamage(int calculatedDamage)
    {
        throw new System.NotImplementedException();
    }
}
