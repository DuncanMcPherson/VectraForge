using System.Reflection;
using System.Windows.Input;
using VectraForge.Commands;
using VectraForge.Models;

namespace VectraForge.ViewModel;

public class AboutDialogViewModel : ViewModelBase, IRequestClose
{
    public event Action<object?>? CloseRequested;

    public string Version { get; } = ResolveVersion();
    public ICommand CloseCommand { get; }

    public AboutDialogViewModel()
    {
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(null));
    }

    private static string ResolveVersion()
    {
        var info = typeof(AboutDialogViewModel).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (string.IsNullOrEmpty(info)) return "unknown";
        var plus = info.IndexOf('+');
        return plus >= 0 ? info[..plus] : info;
    }
}