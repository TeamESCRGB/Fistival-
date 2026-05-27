namespace Coordinator
{
    public class TargetObjectHighlighter : HIghlighterBase
    {
        public void ActivateShader()
        {
            _renderer.sharedMaterial = _on;
        }

        public void DeActivateShader()
        {
            _renderer.sharedMaterial = _off;
        }
    }
}