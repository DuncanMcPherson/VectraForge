using Moq;
using VectraForge.Models;
using VectraForge.Services;
using VectraForge.Services.Interfaces;
using VectraForge.ViewModel;
using VectraForge.Windows.Dialogs;

namespace VectraForge.Tests.ViewModel;

[TestFixture]
public class StartWindowViewModelTests
{
    private Mock<IRecentProjectService> _recentProjectService;
    private Mock<IWindowManager> _windowManager;
    private StartWindowViewModel _sut;

    [SetUp]
    public void Setup()
    {
        _recentProjectService = new Mock<IRecentProjectService>();
        _windowManager = new Mock<IWindowManager>();
        _sut = new StartWindowViewModel(_recentProjectService.Object, _windowManager.Object);
    }

    [TestFixture]
    public class OnClosing : StartWindowViewModelTests
    {
        [Test]
        public void ShouldUnsubscribeFromEventsAndUnregisterDialogOwner()
        {
            _sut.OnClosing();
            _windowManager.Verify(x => x.UnregisterDialogOwner(_sut), Times.Once);
            _recentProjectService.SetupRemove(x => x.RecentProjectsChanged -= It.IsAny<EventHandler<IReadOnlyList<RecentProject>>>());
        }
    }

    [TestFixture]
    public class ContinueWithoutCodeCommand : StartWindowViewModelTests
    {
        [Test]
        public void ShouldCallWindowManagerShowProjectWindowWithNull()
        {
            _sut.ContinueWithoutCodeCommand.Execute(null);
            _windowManager.Verify(x => x.ShowProjectWindow(null), Times.Once);
        }
    }

    [TestFixture]
    public class OpenRecentProject : StartWindowViewModelTests
    {
        [Test]
        public void ShouldNotCallAnythingWhenRecentIsNull()
        {
            _sut.OpenRecentProjectCommand.Execute(null);
            _recentProjectService.Verify(x => x.NotifyProjectOpened(It.IsAny<RecentProject>()), Times.Never);
            _windowManager.Verify(x => x.ShowProjectWindow(It.IsAny<ProjectContext>()), Times.Never);
        }

        [Test]
        public void
            ShouldCallRecentProjectServiceNotifyProjectOpenedAndWindowManagerShowProjectWindowWhenRecentIsNotNull()
        {
            var recent = new RecentProject
            {
                Kind = ProjectKind.Folder,
                LastOpened = DateTimeOffset.UtcNow,
                Name = "SomeTest",
                Path = "Some/Path"
            };
            _sut.OpenRecentProjectCommand.Execute(recent);
            _recentProjectService.Verify(x => x.NotifyProjectOpened(It.IsAny<RecentProject>()), Times.Once);
            _windowManager.Verify(x => x.ShowProjectWindow(It.IsAny<ProjectContext>()), Times.Once);
        }
    }

    [TestFixture]
    public class OpenProject : StartWindowViewModelTests
    {
        [Test]
        public void ShouldNotCallWindowManagerShowProjectWindowWhenDialogResultIsNull()
        {
            _windowManager.Setup(x => x.ShowDialogWindow<ProjectPickerDialog, ProjectSourceSelection>(_sut))
                .ReturnsAsync((ProjectSourceSelection?)null);
            _sut.OpenProjectCommand.Execute(null);
            _windowManager.Verify(x => x.ShowProjectWindow(It.IsAny<ProjectContext>()), Times.Never);
        }

        [Test]
        public void ShouldNotCallWindowManagerShowProjectWindowWhenDialogResultIsCancelled()
        {
            var selection = new ProjectSourceSelection(ProjectSourceKind.Cancelled, "Some/Path");
            _windowManager.Setup(x => x.ShowDialogWindow<ProjectPickerDialog, ProjectSourceSelection>(_sut))
                .ReturnsAsync(selection);
            _sut.OpenProjectCommand.Execute(null);
            _windowManager.Verify(x => x.ShowProjectWindow(It.IsAny<ProjectContext>()), Times.Never);
        }

        [Test]
        public void ShouldNotCallWindowManagerShowProjectWindowWhenDialogResultHasNoPath()
        {
            var selection = new ProjectSourceSelection(ProjectSourceKind.Folder, string.Empty);
            _windowManager.Setup(x => x.ShowDialogWindow<ProjectPickerDialog, ProjectSourceSelection>(_sut))
                .ReturnsAsync(selection);
            _sut.OpenProjectCommand.Execute(null);
            _windowManager.Verify(x => x.ShowProjectWindow(It.IsAny<ProjectContext>()), Times.Never);
        }
        
        [TestCase(ProjectSourceKind.Folder)]
        [TestCase(ProjectSourceKind.File)]
        public void ShouldCallWindowManagerShowProjectWindowWhenDialogResultIsAProject(ProjectSourceKind kind)
        {
            var selection = new ProjectSourceSelection(kind, "Some/Path");
            _windowManager.Setup(x => x.ShowDialogWindow<ProjectPickerDialog, ProjectSourceSelection>(_sut))
                .ReturnsAsync(selection);
            _sut.OpenProjectCommand.Execute(null);
            _windowManager.Verify(x => x.ShowProjectWindow(It.IsAny<ProjectContext>()), Times.Once);
            _windowManager.Verify(x => x.ShowProjectWindow(It.Is<ProjectContext>(c => c.RootPath == "Some/Path")), Times.Once);
        }
    }
}