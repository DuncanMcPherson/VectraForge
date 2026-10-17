using VectraForge.Commands;

namespace VectraForge.Tests.Commands;

[TestFixture]
public class AsyncRelayCommandTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Execute_WhileRunning_BlocksReentry_AndRestoresAfter()
    {
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var finished = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        var raised = 0;

        var cmd = new AsyncRelayCommand(() =>
        {
            calls++;
            return gate.Task;
        });
        cmd.CanExecuteChanged += (_, _) =>
        {
            if (++raised == 2)
            {
                finished.SetResult();
            }
        };
        cmd.Execute(null);
        Assert.That(cmd.CanExecute(null), Is.False);
        
        cmd.Execute(null);
        Assert.That(calls, Is.EqualTo(1));

        gate.SetResult();
        await finished.Task.WaitAsync(TimeSpan.FromSeconds(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(cmd.CanExecute(null), Is.True);
            Assert.That(raised, Is.EqualTo(2));
        }
    }
}