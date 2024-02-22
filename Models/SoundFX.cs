using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace SpaceInvadersMVVM.Models;

public class SoundFx : LibVLC
{
    private string File { get; }
    private Media Media { get; }
    private MediaPlayer MediaPlayer { get; set; }

    public SoundFx(string file)
    {
        File = file;
        Media = new Media(this, File);
        MediaPlayer = new MediaPlayer(Media);
        Log += null;
        InitializeMediaPlayer();

    }
    
    private void InitializeMediaPlayer()
    {
        var libVlc = new LibVLC();
        MediaPlayer = new MediaPlayer(libVlc);
    }

    public void Play(int volume)
    {
        string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/Audio", File);

        if (MediaPlayer.State == VLCState.Playing)
        {
            return;
        }
        
        MediaPlayer.Stop(); // Para a reprodução atual, se houver

        Task.Run(() =>
        {
            // Inicia a reprodução em uma nova thread
            var media = new Media(this, new Uri(outputPath));
            MediaPlayer.Play(media);
            MediaPlayer.Play();

            // Mantém a thread em execução enquanto a reprodução estiver ativa
            while (MediaPlayer.State == VLCState.Playing)
            {
                if (MediaPlayer.Volume != volume)
                {
                    MediaPlayer.Volume = volume;
                }

                Thread.Sleep(100); // Aguarda um curto período para evitar consumo excessivo de CPU
            }
        });
    }

    public void PlayInLoop(int volume)
    {
        string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/Audio", File);

        MediaPlayer.Stop(); // Pare a reprodução atual, se houver
        var media = new Media(this, new Uri(outputPath));
        media.AddOption(":input-repeat=-1"); // Configura a repetição em loop

        MediaPlayer.Volume = volume; // Define o volume

        Task.Run(() =>
        {
            // Inicia a reprodução em uma nova thread
            MediaPlayer.Play(media);

            // Mantém a thread em execução enquanto a reprodução estiver em loop
            while (MediaPlayer.State == VLCState.Playing)
            {
                Thread.Sleep(100); // Aguarda um curto período para evitar consumo excessivo de CPU
            }
        });
    }


    public void SetVolume(int volume)
    {
        if (volume < 0 || volume > 100)
        {
            throw new ArgumentException("O volume deve estar entre 0 e 100.");
        }

        MediaPlayer.Volume = volume;
    }



    public void Stop() => MediaPlayer.Stop();
    
    public VLCState State => MediaPlayer.State;
}