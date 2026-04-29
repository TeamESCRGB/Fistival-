namespace InputHandler
{
    public interface IHorizontalMovementInputHandler
    {
        public void OnLeftMovementInputEvent(bool pressed);
        public void OnRightMovementInputEvent(bool pressed);
    }
}
