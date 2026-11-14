using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

[ExcludeFromCodeCoverage]
public sealed class ProjectContextAccessor : IProjectContextAccessor
{
    public ProjectContext? Current 
    { 
        get; 
        set => SetField(ref field, value);
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}