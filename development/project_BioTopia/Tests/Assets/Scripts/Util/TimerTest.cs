using Timer = Util.Timer;
using Moq;

namespace Tests.Assets.Scripts.Util;

[TestFixture]
[TestOf(typeof(Timer))]
public class TimerTest
{
    private Mock<IMock> _mock;

    [SetUp]
    public void Setup()
    {
        _mock = new Mock<IMock>();
    }

    [Test]
    public void OncePerSecond_LineNumbers()
    {
        // arrange
        var timer = new Timer();

        // act
        // those both get called because they are not on the same line.
        timer.OncePerSecond(_mock.Object.DoNothing);
        timer.OncePerSecond(_mock.Object.DoNothing);
        // this gets called once because they are on the same line
        // @formatter:off
        timer.OncePerSecond(_mock.Object.DoNothing); timer.OncePerSecond(_mock.Object.DoNothing);
        // @formatter:on
        // this one gets called once
        for (var i = 0; i < 10; ++i)
        {
            timer.OncePerSecond(_mock.Object.DoNothing);
        }

        // assert
        _mock.Verify(foo => foo.DoNothing(), Times.Exactly(4));
    }
}

public interface IMock
{
    public void DoNothing();
}