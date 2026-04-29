namespace InputHandler
{
    public interface IJumpsMovementInputHandler
    {
        public void OnJumpMovementInputEvent(bool pressed);
        public void OnDownJumpMovementInputEvent(bool pressed);
    }
}
