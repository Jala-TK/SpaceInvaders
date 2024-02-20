using LibVLCSharp.Shared;
using System;

namespace SpaceInvadersMVVM.Models
{
    public class SoundFx
    {
        private readonly MediaPlayer _mediaPlayer;
        private readonly LibVLC _libVLC;
        //internal object Media;
        private Media _media;
        internal Action<object?, EventArgs> AudioPlaybackEnded;
        internal VLCState State;

        public SoundFx(string file)
        {
            var libVLC = new LibVLC();
            _media = new Media(libVLC, file);
            _mediaPlayer = new MediaPlayer(_media);
            _mediaPlayer.EndReached += LoopAudio;
        }

        public void Play()
        {
            _mediaPlayer.Play();
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
        }

        public void SetVolume(float volume)
        {
            _mediaPlayer.Volume = (int)volume;
        }

        private void LoopAudio(object? sender, EventArgs e)
        {
            // Correção: Verificar se o estado do media player é Ended antes de reiniciar
            if (_mediaPlayer.State == VLCState.Ended)
            {
                // Correção: Parar a reprodução antes de reiniciar
                _mediaPlayer.Stop();

#pragma warning disable CS8602 // Desreferência de uma referência possivelmente nula.
                var media = new Media(_libVLC, _mediaPlayer.Media.Mrl);
#pragma warning restore CS8602 // Desreferência de uma referência possivelmente nula.
                _mediaPlayer.Media = media;
                _mediaPlayer.Play();
            }
        }
    }
}
