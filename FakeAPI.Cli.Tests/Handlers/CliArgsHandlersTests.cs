using System.Text;
using Bogus;
using FluentAssertions;
using Moq;
using Raccoon.Ninja.FakeAPI.Cli.Enums;
using Raccoon.Ninja.FakeAPI.Cli.Handlers;
using Raccoon.Ninja.FakeAPI.Cli.Interfaces;
using Raccoon.Ninja.FakeAPI.Cli.ValueObjects;

namespace FakeAPI.Cli.Tests.Handlers;

public class CliArgsHandlersTests
{
    private readonly CliOptions _defaultOptions;

    public CliArgsHandlersTests()
    {
        _defaultOptions = new CliOptions(200, new List<string>(), FileReturnOption.None, 5000, "https://localhost");
    }

    [Theory]
    [MemberData(nameof(GetDefaultArgsData))]
    public void CliArgsHandlers_Ctor_DefaultOptions(string[] args)
    {
        //Arrange
        var handler = Build();

        //Act
        var parsed = handler.Parse(args);

        //Assert
        parsed.Should().BeEquivalentTo(_defaultOptions);
    }

    [Theory]
    [MemberData(nameof(GetDefaultArgsData))]
    public void CliArgsHandlers_CustomStatusCode_AlteredDefaultOptions(string[] args)
    {
        //Arrange
        var handler = Build();
        handler.SetDefaultStatusCodeReturn(418);
        var expectedOptions = _defaultOptions with { StatusCode = 418 };
        
        //Act
        var parsed = handler.Parse(args);

        //Assert
        parsed.Should().BeEquivalentTo(expectedOptions);
    }

    [Theory]
    [MemberData(nameof(GetFileOptionArgsData))]
    public void CliArgsHandlers_FileOption_ParsedValue(string[] args, FileReturnOption expectedReturnOption)
    {
        //Arrange
        var handler = Build();

        //Act
        var parsed = handler.Parse(args);

        //Assert
        parsed.FileOption.Should().Be(expectedReturnOption);
    }

    [Fact]
    public void CliArgsHandlers_NoFilesInCmdLine_EmptyFileList()
    {
        //Arrange
        var handler = Build();

        //Act
        var parsed = handler.Parse(new []{"-foo=bar"});

        //Assert
        parsed.Files.Should().BeEmpty();
        parsed.HasReturnContent.Should().BeFalse();
    }
    
    [Fact]
    public void CliArgsHandlers_MissingFilesInCmdLine_EmptyFileList()
    {
        //Arrange
        var fileHandlerMock = new Mock<IFileHandler>();
        fileHandlerMock
            .Setup(fh => fh.Exists(It.IsAny<string>())).Returns(false);
        var handler = Build(fileHandlerMock.Object);
        var filesPassedInCmdLine = new[] { "-file=./foo.txt", "-file=./foo2.txt" }; 
        
        //Act
        var parsed = handler.Parse(filesPassedInCmdLine);

        //Assert
        parsed.Files.Should().BeEmpty();
        parsed.HasReturnContent.Should().BeFalse();
        fileHandlerMock
            .Verify(fh => 
                fh.Exists(It.IsAny<string>()), Times.Exactly(filesPassedInCmdLine.Length));
    }
    
    [Fact]
    public void CliArgsHandlers_MixedFilesInCmdLine_OnlyExistingFiles()
    {
        //Arrange
        var fileHandlerMock = new Mock<IFileHandler>();
        const string existingFile = "./foo2.txt";
        
        fileHandlerMock
            .Setup(fh => fh.Exists(It.IsAny<string>()))
            .Returns<string>(filename => filename == existingFile);
        
        var handler = Build(fileHandlerMock.Object);
        var filesPassedInCmdLine = new[] { "-file=./foo.txt", $"-file={existingFile}" }; 
        
        //Act
        var parsed = handler.Parse(filesPassedInCmdLine);

        //Assert
        parsed.HasReturnContent.Should().BeTrue();
        parsed.Files.Should().ContainSingle();
        parsed.Files[0].Should().Be(existingFile);
        fileHandlerMock
            .Verify(fh => 
                fh.Exists(It.IsAny<string>()), Times.Exactly(filesPassedInCmdLine.Length));
    }
    
    [Theory]
    [MemberData(nameof(GetTestFilenames))]
    public void CliArgsHandlers_AllFilesExist_ParsedValue(string[] args)
    {
        //Arrange
        var fileHandlerMock = new Mock<IFileHandler>();
        fileHandlerMock
            .Setup(fh => fh.Exists(It.IsAny<string>()))
            .Returns(true);
        var handler = Build(fileHandlerMock.Object);
        
        //Act
        var parsed = handler.Parse(args);

        //Assert
        parsed.Files.Should().HaveCount(args.Length);
        parsed.HasReturnContent.Should().BeTrue();
    }
    
    
    #region Test Helpers

    public static CliArgsHandler Build(IFileHandler fileHandlerReplacement = null)
    {
        return new CliArgsHandler(fileHandlerReplacement ?? new FileHandler());
    }
    
    public static IEnumerable<object[]> GetDefaultArgsData()
    {
        yield return new object[] { null };
        yield return new object[] { new List<string>().ToArray() };
    }
    
    public static IEnumerable<object[]> GetFileOptionArgsData()
    {
        yield return new object[] { new [] {"-fileOption=None"}, FileReturnOption.None };
        yield return new object[] { new [] {"-fileOption=Fixed"}, FileReturnOption.Fixed };
        yield return new object[] { new [] {"-fileOption=Cycle"}, FileReturnOption.Cycle };
        yield return new object[] { new [] {"-fileOption=none"}, FileReturnOption.None };
        yield return new object[] { new [] {"-fileOption=fixed"}, FileReturnOption.Fixed };
        yield return new object[] { new [] {"-fileOption=cycle"}, FileReturnOption.Cycle };
        yield return new object[] { new [] {"-fileoption=none"}, FileReturnOption.None };
        yield return new object[] { new [] {"-fileoption=fixed"}, FileReturnOption.Fixed };
        yield return new object[] { new [] {"-fileoption=cycle"}, FileReturnOption.Cycle };

        var options = new Dictionary<string, FileReturnOption>
        {
            {"-fileoption=none", FileReturnOption.None},
            {"-fileoption=fixed", FileReturnOption.Fixed},
            {"-fileoption=cycle", FileReturnOption.Cycle}
        };
        
        foreach (var (args, expected) in options)
        {
            var textSb = new StringBuilder(args);
        
            for (var i = 0; i < args.Length; i++)
            {
                textSb[i] = char.ToUpper(textSb[i]);
                yield return new object[] { new [] {textSb.ToString()}, expected };
            }
            
            for (var i = 0; i < args.Length; i++)
            {
                textSb[i] = char.ToLower(textSb[i]);
                yield return new object[] { new [] {textSb.ToString()}, expected };
            }
            
            for (var i = 0; i < args.Length; i++)
            {
                var textSb2 = new StringBuilder(args); 
                textSb2[i] = char.ToUpper(textSb2[i]);
                yield return new object[] { new [] {textSb2.ToString()}, expected };
            }
        }
    }

    public static IEnumerable<object[]> GetTestFilenames()
    {
        const int total = 100;
        const int qtyPerCmdLine = 10;
        var generator = new Faker();

        for (var i = 0; i < total; i++)
        {
            var args1 = Enumerable.Range(0, qtyPerCmdLine).Select(_ => $"-file={generator.System.FilePath()}").ToList();
            yield return new object[] { args1 };
        }
    }
    #endregion
}