using System.Text.Json;
using System.Text.Json.Serialization;

namespace CNPJValidator
{
    public sealed class CnpjJsonConverter : JsonConverter<Cnpj>
    {
        private static Cnpj Parse(string s)
        {
            var result = Cnpj.Create(s);
            if (result.IsFailure)
            {
                throw new JsonException($"Invalid CNPJ format: {s}");
            }
            return result.Value;
        }

        public override Cnpj Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Parse(reader.GetString()!);
        
        public override void Write(Utf8JsonWriter writer, Cnpj value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Value);
    }
}
