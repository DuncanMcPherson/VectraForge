using System.Text;
using System.Windows.Input;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.Input;
using VectraForge.Services.Interfaces;

namespace VectraForge.ViewModel;

public class EditorFileViewModel : ViewModelBase
{
    private const long MaxBytes = 5 * 1024 * 1024;
    private readonly IAtomicFileStore _fileStore;
    private int _changeCount;
    
    public string FilePath { get; }
    public TextDocument? Document { get; private set; }
    public ICommand SaveCommand { get; }

    public string DisplayTitle => Path.GetFileName(FilePath) + (IsDirty ? " •" : "");

    public bool IsDirty
    {
        get;
        private set
        {
            if (SetField(ref field, value))
                OnPropertyChanged(nameof(DisplayTitle));
        }
    }

    public string? Message
    {
        get;
        private set
        {
            if (SetField(ref field, value))
                OnPropertyChanged(nameof(HasMessage));
        }
    }

    public bool HasMessage => Message is not null;

    private EditorFileViewModel(string filePath, IAtomicFileStore fileStore)
    {
        FilePath = filePath;
        _fileStore = fileStore;
        SaveCommand = new AsyncRelayCommand(() => SaveAsync());
    }

    public static async Task<EditorFileViewModel> LoadAsync(string filePath, IAtomicFileStore fileStore)
    {
        var vm = new EditorFileViewModel(filePath, fileStore);
        try
        {
            var text = await Task.Run(() => ReadText(filePath));
            vm.Document = new TextDocument(text);
            vm.Document.TextChanged += (_, _) => vm.OnTextChanged();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
        {
            vm.Message = ex.Message;
        }
        return vm;
    }

    private static string ReadText(string path)
    {
        if (new FileInfo(path).Length > MaxBytes)
            throw new InvalidDataException($"File is larger than {MaxBytes / (1024 * 1024)} MB.");
        var bytes = File.ReadAllBytes(path);
        if (bytes.AsSpan(0, Math.Min(bytes.Length, 8192)).Contains((byte)0))
            throw new InvalidDataException("File looks binary (or isn't UTF-8) and can't be shown yet.");
        var skip = bytes.AsSpan().StartsWith(Encoding.UTF8.Preamble) ? Encoding.UTF8.Preamble.Length : 0;
        try
        {
            return new UTF8Encoding(false, true).GetString(bytes, skip, bytes.Length - skip);
        }
        catch (DecoderFallbackException)
        {
            throw new InvalidDataException("File isn't valid UTF-8; other encodings aren't supported yet.");
        }
    }

    private void OnTextChanged()
    {
        _changeCount++;
        IsDirty = true;
    }

    public async Task<bool> SaveAsync()
    {
        if (Document is null) return true;
        var text = Document.Text;
        var seen = _changeCount;

        try
        {
            await _fileStore.WriteAsync(FilePath, text);
            Message = null;
            if (_changeCount != seen) return false;
            IsDirty = false;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Message = $"Couldn't save: {ex.Message}";
            return false;
        }
    }
}