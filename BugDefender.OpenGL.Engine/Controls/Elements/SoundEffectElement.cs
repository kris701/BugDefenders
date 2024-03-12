namespace BugDefender.OpenGL.Engine.Controls.Elements
{
    public class SoundEffectElement
    {
        public Guid SoundEffect { get; set; }
        private readonly IWindow _parent;

        public SoundEffectElement(Guid soundEffect, IWindow parent)
        {
            SoundEffect = soundEffect;
            _parent = parent;
        }

        public SoundEffectElement(IWindow parent)
        {
            _parent = parent;
        }

        public void Trigger()
        {
            if (SoundEffect != Guid.Empty)
                _parent.AudioController.PlaySoundEffectOnce(SoundEffect);
        }
    }
}
