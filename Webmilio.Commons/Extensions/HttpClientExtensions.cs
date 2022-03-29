using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webmilio.Commons.Extensions
{
    public static class HttpClientExtensions
    {
        /// <summary>Downloads an image from the given Uri into the specified file. Will overwrite any files at all.</summary>
        /// <param name="client"></param>
        /// <param name="uri"></param>
        /// <param name="destination"></param>
        /// <returns><c>true</c> if the image was successfully downloaded; <c>false</c> otherwise.</returns>
        public static async Task<bool> Download(this HttpClient client, Uri uri, FileInfo destination)
        {
            HttpResponseMessage response = await client.GetAsync(uri);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return false;
            }

            using BinaryReader inputStream = new(await response.Content.ReadAsStreamAsync());

#if RELEASE_NS2_0
            using BinaryWriter outputStream = new(destination.Open(FileMode.Create));
#else
            await using BinaryWriter outputStream = new(destination.Open(FileMode.Create));
#endif

            bool shouldRead = true;
            while (shouldRead)
            {
                try
                {
                    outputStream.Write(inputStream.ReadByte());
                }
                catch
                {
                    shouldRead = false;
                }
            }

            outputStream.Flush();

            return true;
        }
    }
}