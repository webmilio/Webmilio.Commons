using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Webmilio.Commons.Configurations;

namespace Webmilio.Commons.NETCore
{
    public class JsonConfiguration<T> : Configuration<T> where T : JsonConfiguration<T>, new()
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };


        public async Task<T> Update(Action<T> modify)
        {
            var t = (T) this;
            modify(t);

            await Save();
            return t;
        }


        public Task Save()
        {
            return Save(Path);
        }

        public override async Task Save(string path)
        {
            await using FileStream fw = File.Open(path, FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(fw, this);
        }

        public static async Task<T> Load(string path)
        {
            bool exists = File.Exists(path);
            await using FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            T instance = default;

            if (exists)
                try
                {
                    instance = await JsonSerializer.DeserializeAsync<T>(stream);
                }
                catch (JsonException)
                {
                    // We don't do anything with the exception, we go ahead and create a new instance.
                }

            instance ??= new T();

            stream.SetLength(0);
            await JsonSerializer.SerializeAsync(stream, instance, _jsonOptions);

            instance.Path = path;
            return instance;
        }


        [JsonIgnore]
        public string Path { get; set; }
    }
}