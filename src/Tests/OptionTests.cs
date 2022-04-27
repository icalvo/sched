using FluentAssertions;
using Scheduler.Options;
using Xunit;

namespace Scheduler.Tests;

public class OptionTests
{
    [Fact]
    public void MapOptional_ReturnsNone()
    {
        Option<int> subject = Option.None;
        subject.MapOptional(i => Option.Some("Hi" + i)).Should().Be(Option.None);
    }

    
    [Fact]
    public void MapOptional_WhenSome_ReturnsMapped()
    {
        Option<int> subject = Option.Some(3);
        subject.MapOptional(i => Option.Some("Hi" + i)).Should().Be(Option.Some("Hi3"));
    } 
    
    [Fact]
    public void Map_WhenNone_ReturnsNone()
    {
        Option<int> subject = Option.None;
        subject.Map(i => "Hi" + i).Should().Be(Option.None);
    }
    
    [Fact]
    public void Map_WhenSome_ReturnsMapped()
    {
        Option<int> subject = Option.Some(3);
        subject.Map(i => "Hi" + i).Should().Be(Option.Some("Hi3"));
    }    
}