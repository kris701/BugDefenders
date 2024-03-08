using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace BugDefender.OpenGL.Engine.Audio
{
    public class AudioController
    {
        private readonly Dictionary<Guid, SongDefinition> _songs = new Dictionary<Guid, SongDefinition>();
        private readonly Dictionary<Guid, SoundEffectDefinition> _soundEffects = new Dictionary<Guid, SoundEffectDefinition>();
        private readonly Dictionary<Guid, SoundEffectInstance> _instances = new Dictionary<Guid, SoundEffectInstance>();
        private string _playing = "";
        private readonly ContentManager _contentManager;

        public AudioController(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public void LoadSong(SongDefinition item)
        {
            if (_songs.ContainsKey(item.ID))
                _songs.Remove(item.ID);
            item.SetContent(_contentManager.Load<Song>(item.Content));
            _songs.Add(item.ID, item);
        }

        public void LoadSoundEffect(SoundEffectDefinition item)
        {
            if (_soundEffects.ContainsKey(item.ID))
                _soundEffects.Remove(item.ID);
            item.SetContent(_contentManager.Load<SoundEffect>(item.Content));
            _soundEffects.Add(item.ID, item);
        }

        public void PlaySong(Guid id)
        {
            if (!_songs.ContainsKey(id))
                return;
            var song = _songs[id];
            if (song.Content == _playing)
                return;
            _playing = song.Content;
            MediaPlayer.Stop();
            MediaPlayer.Play(song.GetLoadedContent());
        }

        public void StopSong()
        {
            _playing = "";
            MediaPlayer.Stop();
        }

        public Guid PlaySoundEffect(Guid id)
        {
            if (!_soundEffects.ContainsKey(id))
                return Guid.Empty;
            var newEffect = Guid.NewGuid();
            _instances.Add(newEffect, _soundEffects[id].GetLoadedContent().CreateInstance());
            _instances[newEffect].Volume = SoundEffect.MasterVolume;
            _instances[newEffect].IsLooped = true;
            _instances[newEffect].Play();
            return newEffect;
        }

        public void PlaySoundEffectOnce(Guid id)
        {
            if (!_soundEffects.ContainsKey(id))
                return;
            _soundEffects[id].GetLoadedContent().Play();
        }

        public void StopSoundEffect(Guid id)
        {
            if (!_instances.ContainsKey(id))
                return;
            _instances[id].Stop();
            _instances.Remove(id);
        }

        public void PauseSounds()
        {
            foreach (var key in _instances.Keys)
                _instances[key].Pause();
            MediaPlayer.Pause();
        }

        public void ResumeSounds()
        {
            foreach (var key in _instances.Keys)
                _instances[key].Resume();
            MediaPlayer.Resume();
        }

        public void StopSounds()
        {
            foreach (var key in _instances.Keys)
                _instances[key].Stop();
            _instances.Clear();
            MediaPlayer.Stop();
            _playing = "";
        }
    }
}
