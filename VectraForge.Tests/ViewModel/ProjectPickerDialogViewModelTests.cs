using Avalonia.Platform.Storage;
using Moq;
using VectraForge.Models;
using VectraForge.Services.Interfaces;
using VectraForge.ViewModel;

namespace VectraForge.Tests.ViewModel;

[TestFixture]
public class ProjectPickerDialogViewModelTests
{
    private Mock<IProjectSourcePicker> _projectSourcePicker;
    private ProjectPickerDialogViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        _projectSourcePicker = new Mock<IProjectSourcePicker>();
        _sut = new ProjectPickerDialogViewModel(_projectSourcePicker.Object);
    }

    [TestFixture]
    public class Constructor : ProjectPickerDialogViewModelTests
    {
        [Test]
        public void ShouldInitializeProjectSourcePickerViewModelCommands()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(_sut.PickFileCommand, Is.Not.Null);
                Assert.That(_sut.PickFolderCommand, Is.Not.Null);
            }
        }
    }
    
    [TestFixture]
    public class PickFileCommand : ProjectPickerDialogViewModelTests
    {
        [Test]
        public void ShouldHandleCancellation()
        {
            _sut.CloseRequested += (arg) =>
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(arg, Is.Not.Null);
                    Assert.That(arg, Is.InstanceOf<ProjectSourceSelection>());
                    if (arg is null)
                        return;
                    var projectSourceSelection = (ProjectSourceSelection)arg;
                    Assert.That(projectSourceSelection.Kind, Is.EqualTo(ProjectSourceKind.Cancelled));
                    Assert.That(projectSourceSelection.Path, Is.Empty);
                }
            };
            _projectSourcePicker.Setup(s => s.PickFileAsync())
                .ReturnsAsync((IStorageItem?)null);
            _sut.PickFileCommand!.Execute(null);
        }

        [Test]
        public void ReturnsWithSourceKindFileAndPath()
        {
            _sut.CloseRequested += arg =>
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(arg, Is.Not.Null);
                    Assert.That(arg, Is.InstanceOf<ProjectSourceSelection>());
                    if (arg is null)
                        return;
                    var projectSourceSelection = (ProjectSourceSelection)arg;
                    Assert.That(projectSourceSelection.Kind, Is.EqualTo(ProjectSourceKind.File));
                    Assert.That(projectSourceSelection.Path, Is.Not.Empty);
                }
            };
            var file = new Mock<IStorageFile>();
            file.Setup(x => x.Path)
                .Returns(new Uri("file://Some/Path"));
            _projectSourcePicker.Setup(x => x.PickFileAsync())
                .ReturnsAsync(file.Object);
            _sut.PickFileCommand!.Execute(null);
        }
    }

    [TestFixture]
    public class PickFolderCommand : ProjectPickerDialogViewModelTests
    {
        [Test]
        public void ShouldHandleCancellation()
        {
            _sut.CloseRequested += (arg) =>
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(arg, Is.Not.Null);
                    Assert.That(arg, Is.InstanceOf<ProjectSourceSelection>());
                    if (arg is null)
                        return;
                    var projectSourceSelection = (ProjectSourceSelection)arg;
                    Assert.That(projectSourceSelection.Kind, Is.EqualTo(ProjectSourceKind.Cancelled));
                    Assert.That(projectSourceSelection.Path, Is.Empty);
                }
            };
            _projectSourcePicker.Setup(s => s.PickFolderAsync())
                .ReturnsAsync((IStorageItem?)null);
            _sut.PickFolderCommand!.Execute(null);
        }
        
        [Test]
        public void ReturnsWithSourceKindFileAndPath()
        {
            _sut.CloseRequested += arg =>
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(arg, Is.Not.Null);
                    Assert.That(arg, Is.InstanceOf<ProjectSourceSelection>());
                    if (arg is null)
                        return;
                    var projectSourceSelection = (ProjectSourceSelection)arg;
                    Assert.That(projectSourceSelection.Kind, Is.EqualTo(ProjectSourceKind.Folder));
                    Assert.That(projectSourceSelection.Path, Is.Not.Empty);
                }
            };
            var folder = new Mock<IStorageFolder>();
            folder.Setup(x => x.Path)
                .Returns(new Uri("file://Some/Path"));
            _projectSourcePicker.Setup(x => x.PickFolderAsync())
                .ReturnsAsync(folder.Object);
            _sut.PickFolderCommand!.Execute(null);
        }
    }
}