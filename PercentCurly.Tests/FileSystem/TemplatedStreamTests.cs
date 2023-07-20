using System;
using System.IO;
using System.Text;
using Moq;
using PercentCurly.FileSystem;
using PercentCurly.Templating;
using PercentCurly.Templating.Case;
using Xunit;

namespace PercentCurly.Tests.FileSystem;

public class TemplatedStreamTests
{
    [Fact]
    public void Constructor_WhenStreamIsNull_ThrowsException()
    {
        // Arrange
        Stream stream = null!;
        var placeholderResolver = new Mock<IPlaceholderResolver>().Object;

        // Act
        var exception = Record.Exception(() => new TemplatedStream(stream, placeholderResolver));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_WhenPlaceholderResolverIsNull_ThrowsException()
    {
        // Arrange
        var stream = new MemoryStream();
        IPlaceholderResolver placeholderResolver = null!;

        // Act
        var exception = Record.Exception(() => new TemplatedStream(stream, placeholderResolver));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Read_WhenStreamIsEmpty_ReturnsEmptyStream()
    {
        // Arrange
        var stream = new MemoryStream();
        var placeholderResolver = new Mock<IPlaceholderResolver>().Object;
        var sut = new TemplatedStream(stream, placeholderResolver);
        var expected = new byte[0];
        var actual = new byte[expected.Length];

        // Act
        sut.Read(actual, 0, actual.Length);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Read_WhenStreamContainsText_ReturnsText()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, World!"));
        var placeholderResolver = new Mock<IPlaceholderResolver>().Object;
        var sut = new TemplatedStream(stream, placeholderResolver);
        var expected = Encoding.UTF8.GetBytes("Hello, World!");
        var actual = new byte[expected.Length];

        // Act
        sut.Read(actual, 0, actual.Length);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Read_WhenStreamContainsPlaceholder_ReturnsResolvedPlaceholder()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, %{name}!"));
        var placeholderResolver = new Mock<IPlaceholderResolver>();
        placeholderResolver.Setup(x => x.Resolve("name")).Returns("World");
        var sut = new TemplatedStream(stream, placeholderResolver.Object);
        var expected = Encoding.UTF8.GetBytes("Hello, World!");
        var actual = new byte[expected.Length];

        // Act
        sut.Read(actual, 0, actual.Length);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Read_WhenStreamContainsMultiplePlaceholders_ReturnsResolvedPlaceholders()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, %{name}! My name is %{name}!"));
        var placeholderResolver = new Mock<IPlaceholderResolver>();
        placeholderResolver.Setup(x => x.Resolve("name")).Returns("World");
        var sut = new TemplatedStream(stream, placeholderResolver.Object);
        var expected = Encoding.UTF8.GetBytes("Hello, World! My name is World!");
        var actual = new byte[expected.Length];

        // Act
        sut.Read(actual, 0, actual.Length);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Read_WhenStreamContainsPlaceholderWithInvalidName_ThrowsException()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, %{name}!"));
        var placeholderResolver = new Mock<IPlaceholderResolver>();
        placeholderResolver.Setup(x => x.Resolve("name")).Returns(null as string);
        var sut = new TemplatedStream(stream, placeholderResolver.Object);
        var actual = new byte[100];

        // Act
        var exception = Record.Exception(() => sut.Read(actual, 0, actual.Length));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<Exception>(exception);
        Assert.Equal("Placeholder 'name' could not be resolved.", exception.Message);
    }

    [Fact]
    public void Read_WhenStreamContainsNonTerminatedPlaceholder_ThrowsException()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, %{name!"));
        var placeholderResolver = new Mock<IPlaceholderResolver>();
        placeholderResolver.Setup(x => x.Resolve("name")).Returns("World");
        var sut = new TemplatedStream(stream, placeholderResolver.Object);
        var actual = new byte[100];

        // Act
        var exception = Record.Exception(() => sut.Read(actual, 0, actual.Length));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<Exception>(exception);
        Assert.Equal("Placeholder was not terminated.", exception.Message);
    }

    [Fact]
    public void Read_WhenStreamContainsMultipleLines_ResolvesPlaceholdersOnAllLines()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, %{you}!\nMy name is %{me}!"));
        var placeholderResolver = new Mock<IPlaceholderResolver>();
        placeholderResolver.Setup(x => x.Resolve("you")).Returns("World");
        placeholderResolver.Setup(x => x.Resolve("me")).Returns("Speck");
        var sut = new TemplatedStream(stream, placeholderResolver.Object);
        var expected = Encoding.UTF8.GetBytes("Hello, World!\nMy name is Speck!");
        var actual = new byte[expected.Length];

        // Act
        sut.Read(actual, 0, actual.Length);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Read_WhenCalledInMultipleChunks_ResolvesPlaceholders()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, %{name}!"));
        var placeholderResolver = new Mock<IPlaceholderResolver>();
        placeholderResolver.Setup(x => x.Resolve("name")).Returns("World");
        var sut = new TemplatedStream(stream, placeholderResolver.Object);
        var expected = Encoding.UTF8.GetBytes("Hello, World!");
        var actual = new byte[expected.Length];

        // Act
        sut.Read(actual, 0, 5);
        sut.Read(actual, 5, 5);
        sut.Read(actual, 10, 3);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Read_WhenUsingCaseTransformingPlaceholderResolver_ResolvesPlaceholders()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("public class %{Application Name}"));
        var placeholderResolver = new CaseTransformingPlaceholderResolver(new ICaseStrategy[]
        {
            new DashCaseStrategy(),
            new SnakeCaseStrategy(),
            new TitleCaseStrategy(),
        });
        placeholderResolver.Register("application-name", "MY_COOL_APPLICATION");
        var sut = new TemplatedStream(stream, placeholderResolver);
        var expected = Encoding.UTF8.GetBytes("public class My Cool Application");
        var actual = new byte[expected.Length];

        // Act
        sut.Read(actual, 0, actual.Length);

        // Assert
        Assert.Equal(expected, actual);
    }
}
