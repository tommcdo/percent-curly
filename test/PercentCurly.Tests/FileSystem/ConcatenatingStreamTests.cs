using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PercentCurly.FileSystem;
using PercentCurly.FileSystem.Streaming;
using Xunit;

namespace PercentCurly.Tests.FileSystem;

public class ConcatenatingStreamTests
{
    [Fact]
    public void Constructor_WhenStreamsIsEmpty_ThrowsException()
    {
        // Arrange
        var streams = new List<Stream>();

        // Act
        var exception = Record.Exception(() => new ConcatenatingStream(streams));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void Read_WithOneStream_ReturnsStream()
    {
        // Arrange
        var expected = new byte[] { 1, 2, 3 };
        var streams = new List<Stream> { new MemoryStream(expected) };
        var sut = new ConcatenatingStream(streams);
        var actual = new byte[expected.Length];

        // Act
        sut.Read(actual, 0, actual.Length);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Read_WithTwoStreams_ReturnsConcatenatedStream()
    {
        // Arrange
        var expected = new byte[] { 1, 2, 3, 4, 5, 6 };
        var streams = new List<Stream>
        {
            new MemoryStream(new byte[] { 1, 2, 3 }),
            new MemoryStream(new byte[] { 4, 5, 6 }),
        };
        var sut = new ConcatenatingStream(streams);
        var actual = new byte[expected.Length];

        // Act
        sut.Read(actual, 0, actual.Length);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Read_WhenMovingToNextStream_ReturnsConcatenatedStream()
    {
        // Arrange
        var expected = new byte[] { 1, 2, 3, 4, 5 };
        var streams = new List<Stream>
        {
            new MemoryStream(new byte[] { 1, 2, 3 }),
            new MemoryStream(new byte[] { 4, 5, 6 }),
        };
        var sut = new ConcatenatingStream(streams);
        var actual = new byte[expected.Length];

        // Act
        sut.Read(actual, 0, 2);
        sut.Read(actual, 2, 3);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Read_WhenCountIsLargerThanStream_ReturnsAvailableBytes()
    {
        // Arrange
        var expected = new byte[] { 1, 2, 3, 4, 5 };
        var streams = new List<Stream>
        {
            new MemoryStream(new byte[] { 1, 2, 3 }),
            new MemoryStream(new byte[] { 4, 5, 6 }),
        };
        var sut = new ConcatenatingStream(streams);
        var actual = new byte[20];

        // Act
        sut.Read(actual, 0, 2);
        sut.Read(actual, 2, 10);

        // Assert
        Assert.Equal(expected, actual.Take(expected.Length));
    }

    [Fact]
    public void Read_WhenStartingAfterEndOfLastStream_ReturnsZero()
    {
        // Arrange
        var expected = new byte[] { 1, 2, 3, 4, 5, 6 };
        var streams = new List<Stream>
        {
            new MemoryStream(new byte[] { 1, 2, 3 }),
            new MemoryStream(new byte[] { 4, 5, 6 }),
        };
        var sut = new ConcatenatingStream(streams);
        var actual = new byte[20];

        // Act
        sut.Read(actual, 0, 3);
        sut.Read(actual, 3, 3);
        var lastBytesRead = sut.Read(actual, 6, 3);

        // Assert
        Assert.Equal(expected, actual.Take(expected.Length));
        Assert.Equal(0, lastBytesRead);
    }

    [Fact]
    public async Task ReadAsync_WithMultipleStreams_ReturnsConcatenatedStream()
    {
        // Arrange
        var streams = new List<Stream>
        {
            new MemoryStream(new byte[] { 65, 66, 67 }),
            new MemoryStream(new byte[] { 68, 69, 70 }),
        };
        var sut = new ConcatenatingStream(streams);
        var reader = new StreamReader(sut);

        // Act
        var actual = await reader.ReadToEndAsync();

        // Assert
        Assert.Equal("ABCDEF", actual);
    }
}
