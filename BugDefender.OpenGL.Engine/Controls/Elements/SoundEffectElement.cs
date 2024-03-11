using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.OpenGL.Engine.Controls.Elements
{
    public class SoundEffectElement
    {
        public Guid SoundEffect { get; set; }
        private IWindow _parent;

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
