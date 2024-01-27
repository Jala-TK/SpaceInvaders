namespace SpaceInvadersMVVM.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";
    private int _score = 0;
    public string Score => "Score: " + _score;
}
