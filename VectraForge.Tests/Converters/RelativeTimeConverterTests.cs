using System.Globalization;
using VectraForge.Converters;

namespace VectraForge.Tests.Converters;

[TestFixture]
public class RelativeTimeConverterTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 14, 12, 0, 0, TimeSpan.Zero);

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private static object? Run(object? value, CultureInfo? culture = null)
        => new RelativeTimeConverter(new FixedTimeProvider(Now))
            .Convert(value, typeof(string), null, culture ?? CultureInfo.InvariantCulture);

    [TestCase(0, "Just now")]
    [TestCase(59, "Just now")]
    [TestCase(60, "1 min ago")]
    [TestCase(3599, "59 min ago")]
    [TestCase(3600, "1 hr ago")]
    [TestCase(86399, "23 hr ago")]
    [TestCase(86400, "1 day ago")] // singular
    [TestCase(172800, "2 days ago")] // plural
    [TestCase(604799, "6 days ago")]
    public void Convert_RelativeBuckets(int secondsAgo, string expected)
        => Assert.That(Run(Now.AddSeconds(-secondsAgo)), Is.EqualTo(expected));

    [Test]
    public void Convert_SevenDaysOrMore_FallsBackToAbsoluteDate()
    {
        var dto = Now.AddDays(-7);
        var expected = dto.LocalDateTime.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);

        Assert.That(Run(dto), Is.EqualTo(expected));
    }

    [Test]
    public void Convert_AbsoluteDate_UsesSuppliedCulture()
    {
        var original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            var dto = Now.AddDays(-30);
            var fr = CultureInfo.GetCultureInfo("fr-FR");

            Assert.That(Run(dto, fr), Is.EqualTo(dto.LocalDateTime.ToString("MMM d, yyyy", fr)));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Test]
    public void Convert_NonDateTimeOffset_PassesValueThrough()
    {
        Assert.That(Run("abc"), Is.EqualTo("abc"));
        Assert.That(Run(null), Is.Null);
    }

    [Test]
    public void ConvertBack_Throws()
        => Assert.Throws<NotSupportedException>(() =>
            new RelativeTimeConverter().ConvertBack("x", typeof(DateTimeOffset), null, CultureInfo.InvariantCulture));
}