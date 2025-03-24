using TC.Logging.Sinks;

namespace TC.Logging.Json.Tests;

[TestClass]
public sealed class JsonFileLogSinkTests
{
    [TestMethod]
    public void TestCreatingNewJsonFile()
    {
        if(File.Exists("new.json"))
            File.Delete("new.json");
        using var sink = new JsonFileLogSink("new.json");
        sink.Process(new LogMessage(Severity.Error, "Test", 0, null));

        var bytes = File.ReadAllBytes("new.json");

        var actual1 = bytes[..2];
        var expected1 = "[{"u8.ToArray();
        CollectionAssert.AreEqual(actual1, expected1);

        var actual2 = bytes[^2..];
        var expected2 = "}]"u8.ToArray();
        CollectionAssert.AreEqual(actual2, expected2);
    }

    [TestMethod]
    public void TestAppendingToExistingJsonFile()
    {
        if(File.Exists("existing.json"))
            File.Delete("existing.json");
        using var sink = new JsonFileLogSink("existing.json");
        sink.Process(new LogMessage(Severity.Error, "Test1", 0, null));
        sink.Process(new LogMessage(Severity.Warning, "Test2", 0, null));

        var bytes = File.ReadAllBytes("existing.json");

        var actual1 = bytes[..2];
        var expected1 = "[{"u8.ToArray();
        CollectionAssert.AreEqual(actual1, expected1);

        ReadOnlySpan<byte> middlePart = bytes[2..^2];
        ReadOnlySpan<byte> bytesToSearch = "},\n{"u8;
        var index = middlePart.IndexOf(bytesToSearch);
        Assert.AreNotEqual(index, -1);

        var actual2 = bytes[^2..];
        var expected2 = "}]"u8.ToArray();
        CollectionAssert.AreEqual(actual2, expected2);
    }

    [TestMethod]
    public void TestExtraData()
    {
        if(File.Exists("extra.json"))
            File.Delete("extra.json");
        using var sink = new JsonFileLogSink("extra.json");
        sink.Process(new LogMessage(Severity.Error, "Test", 0, "Extra data of the log message"));
    }

}
