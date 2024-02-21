using LibVLCSharp.Shared;
using System;

namespace SpaceInvadersMVVM.Models
{
    public class SoundFx : LibVLC
    {
        private readonly MediaPlayer _mediaPlayer;
        private Media _media;
        internal Action<object?, EventArgs> AudioPlaybackEnded;

        public SoundFx(AudioManager audioManager)
        {
            string file = audioManager.GetAudioFilePath();

            Console.WriteLine("PASSEI");
            _media = new Media(this, file); // AQUI
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

        public void LoadAudio(string filePath)
        {
            Console.WriteLine($"Carregando áudio: {filePath}");
            _media = new Media(this, filePath);
            _mediaPlayer.Media = _media;
        }


        private void LoopAudio(object? sender, EventArgs e)
        {
            // Correção: Verificar se o estado do media player é Ended antes de reiniciar
            if (_mediaPlayer.State == VLCState.Ended)
            {
                // Correção: Parar a reprodução antes de reiniciar
                _mediaPlayer.Stop();

#pragma warning disable CS8602 // Desreferência de uma referência possivelmente nula.
                var media = new Media(this, _mediaPlayer.Media.Mrl);
#pragma warning restore CS8602 // Desreferência de uma referência possivelmente nula.
                _mediaPlayer.Media = media;
                _mediaPlayer.Play();
            }
        }
    }
}
