using VectraForge.Converters;

namespace VectraForge.Tests.Converters;

[TestFixture]
public class ParentDirectoryConverterTests
{
    [Test]
    public void Convert_ReturnsParentDirectory()
    {
        var converter = new ParentDirectoryConverter();
        var path = Path.Join(Path.GetTempPath(), "some-random-dir");
        var lastIndexOf = path.LastIndexOf(Path.DirectorySeparatorChar);
        var expected = path[..lastIndexOf];
        var result = converter.Convert(path, null!, null, null!);
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void Convert_NullPath_ThrowsArgumentNullException()
    {
        var converter = new ParentDirectoryConverter();
        Assert.Throws<ArgumentNullException>(() => converter.Convert(null!, null!, null, null!));
    }

    [Test]
    public void Convert_NonString_ReturnsUnmodified()
    {
        var converter = new ParentDirectoryConverter();
        var result = converter.Convert(123, null!, null, null!);
        Assert.That(result, Is.EqualTo(123));
    }

    [Test]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new ParentDirectoryConverter();
        Assert.Throws<NotSupportedException>(() => converter.ConvertBack(null!, null!, null, null!));
    }
}