using System.ComponentModel;

namespace VectraForge.Services.Interfaces;

public interface IProjectContextAccessor : INotifyPropertyChanged
{
    ProjectContext? Current { get; set; }
}