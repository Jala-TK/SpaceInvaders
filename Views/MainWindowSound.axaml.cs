using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Platform;
using NAudio.Wave;

namespace SpaceInvadersMVVM.Views;

public partial class MainWindow : Window
{
    private IWavePlayer wavePlayer;
    private AudioFileReader audioFileReader;

    public void PlayAudio(string assetName, float volume, bool loop)
    {
        using (var stream = AssetLoader.Open(new Uri($"avares://SpaceInvadersMVVM/Assets/Audio/{assetName}")))
        {
            if (stream == null)
                throw new InvalidOperationException("Resource not found.");

            // Create a temporary file
            var tempFile = Path.GetTempFileName();
            using (var fileStream = File.Create(tempFile))
            {
                stream.CopyTo(fileStream);
            }
            
            // Play the audio file
            wavePlayer = new WaveOutEvent();
            audioFileReader = new AudioFileReader(tempFile);
            wavePlayer.Init(audioFileReader);
            wavePlayer.Volume = volume;

            
            wavePlayer.PlaybackStopped += (_, _) =>
            {
                if (loop)
                {
                    audioFileReader.Position = 0; // Reinicia a posição do leitor de áudio
                    wavePlayer.Play(); // Reinicia a reprodução
                }
                else
                {
                    audioFileReader.Dispose();
                    wavePlayer.Dispose();
                    File.Delete(tempFile);
                }
            };
            
            wavePlayer.Play();

            // Cleanup after playback is complete
            wavePlayer.PlaybackStopped += (sender, args) =>
            {
                audioFileReader.Dispose();
                wavePlayer.Dispose();
                File.Delete(tempFile);
            };
        }
    }
}