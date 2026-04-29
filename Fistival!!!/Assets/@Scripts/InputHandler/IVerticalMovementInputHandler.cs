namespace InputHandler
{
    public interface IVerticalMovementInputHandler
    {
        public void OnDownMovementInputEvent(bool pressed);
        public void OnUpMovementInputEvent(bool pressed);
    }
}
