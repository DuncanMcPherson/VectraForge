using Moq;
using VectraForge.Models;
using VectraForge.Services;
using VectraForge.Services.Interfaces;

namespace VectraForge.Tests.Services;

[TestFixture]
public class RecentProjectServiceTests
{
    private Mock<IConfigService> _configService;
    private RecentProjectService _sut;

    [SetUp]
    public void Setup()
    {
        _configService = new Mock<IConfigService>();
        _sut = new RecentProjectService(_configService.Object);
        _configService.Setup(c => c.WriteConfigFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
    }

    [TestFixture]
    public class GetRecentProjects : RecentProjectServiceTests
    {
        [Test]
        public async Task ReturnsEmptyArrayWhenConfigFileIsMissing()
        {
            _configService.Setup(x => x.ReadConfigFile("recent_projects.json"))
                .ReturnsAsync(string.Empty);
            var result = await _sut.GetRecentProjects();
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task ReturnsAListWhenConfigFileValid()
        {
            var project = new RecentProject
            {
                Kind = ProjectKind.Folder,
                LastOpened = DateTimeOffset.UtcNow,
                Name = "TestProject",
                Path = "C:\\TestProject"
            };
            var list = new RecentProjectsList { Projects = [project] };
            _configService.Setup(x => x.ReadConfigFile("recent_projects.json"))
                .ReturnsAsync(list.ToJson());
            var result = await _sut.GetRecentProjects();
            Assert.IsNotEmpty(result);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(project));
        }
    }

    [TestFixture]
    public class NotifyProjectOpened : RecentProjectServiceTests
    {
        private Func<IReadOnlyList<RecentProject>> CreateListener()
        {
            List<RecentProject> last = [];
            _sut.RecentProjectsChanged += (_, arg) => last = [.. arg];
            return () => last;
        }

        private RecentProject CreateRecentProject(string name)
        {
            return new RecentProject
            {
                Kind = ProjectKind.Folder,
                LastOpened = DateTimeOffset.UtcNow,
                Name = name,
                Path = $"test/{name}"
            };
        }

        [Test]
        public void NewProject_GoesToFront()
        {
            var latest = CreateListener();
            _sut.NotifyProjectOpened(CreateRecentProject("A"));
            _sut.NotifyProjectOpened(CreateRecentProject("B"));
            
            Assert.That(latest().Select(x => x.Name), Is.EqualTo(["B", "A"]));
        }

        [Test]
        public void ShouldMoveLatestToFrontWithoutDuplicating()
        {
            var latest = CreateListener();
            _sut.NotifyProjectOpened(CreateRecentProject("A"));
            _sut.NotifyProjectOpened(CreateRecentProject("B"));
            _sut.NotifyProjectOpened(CreateRecentProject("A"));
            
            Assert.That(latest().Select(x => x.Name), Is.EqualTo(["A", "B"]));
        }
    }
}