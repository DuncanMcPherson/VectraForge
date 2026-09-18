namespace VectraForge.Models;

public interface IRequestClose
{
    event Action<object?>? CloseRequested;
}