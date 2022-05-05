using System.Collections.Generic;
using System.Collections.Immutable;
using Cronos;
using FluentAssertions;
using Scheduler.ConfigParser;
using Xunit;

namespace Scheduler.Tests;

public class ActionParserTests
{
    [Fact]
    public void ActionParserConvert_AliasFound()
    {
        IActionParser parser = new MockActionParser();

        var aliases = new Dictionary<string, string>
        {
            { "alias1", "my -command" }
        }.ToImmutableDictionary();
        const string commandSpec = "alias1";
        var cronExpression = CronExpression.Parse("* * * * *");
        var result = parser.Convert(new TaskConfig(commandSpec, cronExpression), aliases);

        result.Description.Should().Be("alias1 (my -command)");
    }

    [Fact]
    public void ActionParserConvert_NoAliasFound()
    {
        IActionParser parser = new MockActionParser();

        var aliases = new Dictionary<string, string>
        {
            { "alias1", "my -command" }
        }.ToImmutableDictionary();
        const string commandSpec = "other -command";
        var cronExpression = CronExpression.Parse("* * * * *");
        var result = parser.Convert(new TaskConfig(commandSpec, cronExpression), aliases);

        result.Description.Should().Be("other -command");
    }
}