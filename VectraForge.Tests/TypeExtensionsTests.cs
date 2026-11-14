using VectraForge.Models;
using VectraForge.Services;

namespace VectraForge.Tests;

[TestFixture]
public class TypeExtensionsTests
{
    [TestCase(ProjectSourceKind.Folder, ProjectKind.Folder)]
    [TestCase(ProjectSourceKind.File, ProjectKind.File)]
    public void ShouldConvertToProjectKind(ProjectSourceKind input, ProjectKind expected)
    {
        var result = input.ToProjectKind();
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldThrowErrorOnInvalidKind()
    {
        const ProjectSourceKind input = ProjectSourceKind.Cancelled;
        Assert.Throws<ArgumentOutOfRangeException>(() => input.ToProjectKind());
    }
}