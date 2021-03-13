using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LeaderAnalytics.Core.Serialization.Json
{
    public class NumberToStringConverter : JsonConverter<string>
    {
        public override string Read(
          ref Utf8JsonReader reader,
          Type typeToConvert,
          JsonSerializerOptions options) => reader.GetInt32().ToString();

        public override void Write(
            Utf8JsonWriter writer,
            string Value,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(Value);
    }
}
