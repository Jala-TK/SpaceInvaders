using System;
using System.IO;
using SpaceInvadersMVVM.Models;

public class AudioManager
{
    private string audioFilePath;
    private SoundFx _soundFx;

    public Action<object?, EventArgs> LoopAudio { get; private set; }

    public void PlayAudio(string assetName, float volume, bool loop)
    {
        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string appDirectory = Path.GetDirectoryName(exePath);
        audioFilePath = Path.Combine(appDirectory, "Assets", "Audio", assetName);
        audioFilePath = Path.GetFullPath(audioFilePath);

        Console.WriteLine(audioFilePath);

        // Verifique se o caminho não está vazio
        if (string.IsNullOrEmpty(audioFilePath))
        {
            Console.WriteLine("Caminho do arquivo de áudio é inválido FALA MEU FIO.");
            return;
        }

        if (_soundFx == null)
        {
            _soundFx = new SoundFx(this);
        }

        _soundFx.SetVolume(volume);

        if (loop)
        {
            _soundFx.AudioPlaybackEnded += LoopAudio;
        }

        _soundFx.LoadAudio(audioFilePath); // Carregue o áudio antes de reproduzi-lo
        _soundFx.Play();

        Console.WriteLine(audioFilePath);
    }

    public string GetAudioFilePath()
    {
        return audioFilePath;
    }

}
