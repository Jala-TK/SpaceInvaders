using ReactiveUI;

namespace SpaceInvadersMVVM.ViewModels;

partial class MainWindowViewModel : ViewModelBase
{
    private int _score;
    public string Score => "Score: " + _score;

    public void UpdateScore(int points)
    {
        _score += points;
        this.RaisePropertyChanged(nameof(Score));
    }
}