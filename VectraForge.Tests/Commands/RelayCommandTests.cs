using VectraForge.Commands;

namespace VectraForge.Tests.Commands;

[TestFixture]
public class RelayCommandTests
{
    [Test]
    public void Execute_InvokesDelegateOnce()
    {
        var calls = 0;
        var cmd = new RelayCommand(() => calls++);
        cmd.Execute(null);
        Assert.That(calls, Is.EqualTo(1));
    }

    [Test]
    public void CanExecute_NullPredicate_ReturnsTrue() =>
        Assert.That(new RelayCommand(() => { }).CanExecute(null), Is.True);

    [TestCase(true)]
    [TestCase(false)]
    public void CanExecute_ReflectsPredicate(bool allowed) =>
        Assert.That(new RelayCommand(() => { }, () => allowed).CanExecute(null), Is.EqualTo(allowed));
    
    [Test]
    public void CanExecute_ReevaluatesPredicateOnEveryCall()
    {
        var allowed = false;
        // ReSharper disable once AccessToModifiedClosure
        var cmd = new RelayCommand(() => { }, () => allowed);

        Assert.That(cmd.CanExecute(null), Is.False);
        allowed = true;
        Assert.That(cmd.CanExecute(null), Is.True);
    }

    [Test]
    public void RaiseCanExecuteChanged_RaisesEventWithCommandAsSender()
    {
        var cmd = new RelayCommand(() => { });
        object? sender = null;
        EventArgs? args = null;
        cmd.CanExecuteChanged += (s, e) => { sender = s; args = e; };

        cmd.RaiseCanExecuteChanged();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sender, Is.SameAs(cmd));
            Assert.That(args, Is.SameAs(EventArgs.Empty));
        }
    }

    [Test]
    public void RaiseCanExecuteChanged_NoSubscribers_DoesNotThrow()
        => Assert.DoesNotThrow(() => new RelayCommand(() => { }).RaiseCanExecuteChanged());
}

[TestFixture]
public class RelayCommandOfTTests
{
    [Test]
    public void Execute_PassesParameterToDelegate()
    {
        string? received = null;
        var cmd = new RelayCommand<string>(p => received = p);

        cmd.Execute("hello");

        Assert.That(received, Is.EqualTo("hello"));
    }

    [Test]
    public void CanExecute_PassesParameterToPredicate()
    {
        var cmd = new RelayCommand<int>(_ => { }, p => p > 5);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cmd.CanExecute(10), Is.True);
            Assert.That(cmd.CanExecute(1), Is.False);
        }
    }

    [Test]
    public void Execute_NullParameter_ReferenceType_PassesNull()
    {
        var called = false;
        var received = "sentinel";
        var cmd = new RelayCommand<string>(p => { called = true; received = p; });

        cmd.Execute(null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(called, Is.True);
            Assert.That(received, Is.Null);
        }
    }

    [Test]
    public void Execute_WrongParameterType_ThrowsInvalidCast()
    {
        var cmd = new RelayCommand<int>(_ => { });

        Assert.Throws<InvalidCastException>(() => cmd.Execute("not an int"));
    }
    
    [Test]
    public void CanExecute_NullPredicate_ReturnsTrue() =>
        Assert.That(new RelayCommand<int>(_ => { }).CanExecute(null), Is.True);

    [TestCase(true)]
    [TestCase(false)]
    public void CanExecute_ReflectsPredicate(bool allowed) =>
        Assert.That(new RelayCommand<int>(_ => { }, _ => allowed).CanExecute(0), Is.EqualTo(allowed));

    [Test]
    public void RaiseCanExecuteChanged_RaisesEventWithCommandAsSender()
    {
        var cmd = new RelayCommand<int>(_ => { });
        object? sender = null;
        EventArgs? args = null;
        cmd.CanExecuteChanged += (s, e) => { sender = s; args = e; };

        cmd.RaiseCanExecuteChanged();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sender, Is.SameAs(cmd));
            Assert.That(args, Is.SameAs(EventArgs.Empty));
        }
    }

    [Test]
    public void RaiseCanExecuteChanged_NoSubscribers_DoesNotThrow()
        => Assert.DoesNotThrow(() => new RelayCommand<int>(_ => { }).RaiseCanExecuteChanged());
    
    [Test]
    public void CanExecute_NullParameter_ValueType_UsesDefault()
    {
        var cmd = new RelayCommand<int>(_ => { }, p => p == 0);
        Assert.That(cmd.CanExecute(null), Is.True);
    }
}