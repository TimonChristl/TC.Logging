using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TC.Logging.Sinks
{
    public class JsonFileLogSink : BaseLogSink
    {
        private readonly string filename;
        private readonly JsonSerializerOptions jsonSerializerOptions;

#if NET8_0_OR_GREATER
        private readonly byte[] openingBracketBytes = "["u8.ToArray();
        private readonly byte[] closingBracketBytes = "]"u8.ToArray();
        private readonly byte[] commaBytes = ",\n"u8.ToArray();
#else
        private readonly byte[] openingBracketBytes = new byte[] { (byte)'[' };
        private readonly byte[] closingBracketBytes = new byte[] { (byte)']' };
        private readonly byte[] commaBytes = new byte[] { (byte)',', (byte)'\n' };
#endif

        public JsonFileLogSink(string filename)
        {
            this.filename = filename;
            jsonSerializerOptions = new JsonSerializerOptions
            {  
                PropertyNamingPolicy = null,
                Converters =
                {
                    new System.Text.Json.Serialization.JsonStringEnumConverter(),
                },
            };
        }

        public override void Process(LogMessage logMessage)
        {
#if NET8_0_OR_GREATER
            ObjectDisposedException.ThrowIf(IsDisposed, this);
#else
			if(IsDisposed)
				throw new ObjectDisposedException("TextFileLogSink");
#endif

            using(Stream stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                long insertPosition = 0;
                bool insertComma = true;

                if(stream.Length == 0)
                {
                    stream.Write(openingBracketBytes, 0, count: openingBracketBytes.Length);
                    insertPosition = stream.Length;
                    insertComma = false;
                }
                else
                {
                    byte[] buffer = new byte[64];
                    stream.Seek(Math.Max(0, stream.Length - 64), SeekOrigin.Begin);
                    var bytesRead = stream.Read(buffer, 0, 64);
                    for(var i = bytesRead - 1; i >= 0; i--)
                        if(buffer[i] == ']')
                        {
                            insertPosition = stream.Length - bytesRead + i;
                            break;
                        }
                }

                stream.Seek(insertPosition, SeekOrigin.Begin);
                if(insertComma)
                    stream.Write(commaBytes, 0, commaBytes.Length);
                JsonSerializer.Serialize<JsonLogMessage>(stream, JsonLogMessageFromLogMessage(logMessage), jsonSerializerOptions);
                stream.Write(closingBracketBytes, 0, closingBracketBytes.Length);
            }
        }

        private JsonLogMessage JsonLogMessageFromLogMessage(LogMessage logMessage)
        {
            return new JsonLogMessage
            {
                TimeStamp = logMessage.TimeStamp,
                Severity = logMessage.Severity,
                Text = logMessage.Text,
                NestingDepth = logMessage.NestingDepth,
                ExtraData = logMessage.GetExtraDataAsString(),
            };
        }
    }

    public class JsonLogMessage
    {
        public DateTime TimeStamp { get; set; }

        public Severity Severity { get; set; }

#if NET8_0_OR_GREATER
       public string? Text { get; set; }
#else
        public string Text { get; set; }
#endif

        public int NestingDepth { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
#if NET8_0_OR_GREATER
        public object? ExtraData { get; set; }
#else
        public object ExtraData { get; set; }
#endif
    }
}
