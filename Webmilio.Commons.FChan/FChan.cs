using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Webmilio.Commons.FChan
{
    public class FChan
    {
        public const string
            FChanCDNUri = "https://a.4cdn.org",
            BoardsUri = "boards.json",
            ThreadsUri = "{0}/threads.json",
            PostUri = "{0}/thread/{1}.json";


        private static readonly HttpClient _client = new HttpClient();


        public static async Task<Board[]> GetBoards()
        {
            var json = await _client.GetStringAsync($"{FChanCDNUri}/{BoardsUri}");
            return JsonSerializer.Deserialize<Board.Response>(json).boards;
        }

        public static async Task<Thread.Response[]> GetThreads(string board)
        {
            var json = await _client.GetStringAsync($"{FChanCDNUri}/{string.Format(ThreadsUri, board)}");
            var responses = JsonSerializer.Deserialize<Thread.Response[]>(json);

            Do(responses, r => Do(r.threads, t => t.Board = board));
            return responses;
        }

        public static async Task<List<Thread>> GetThreadsPageless(string board)
        {
            var threads = new List<Thread>();
            Do(await GetThreads(board), r => threads.AddRange(r.threads));

            return threads;
        }

        public static async Task<Post[]> GetPosts(string board, ulong thread)
        {
            var json = await _client.GetStringAsync($"{FChanCDNUri}/{string.Format(PostUri, board, thread)}");
            var posts = JsonSerializer.Deserialize<Post.Response>(json).posts;

            Do(posts, p =>
            {
                p.Board = board;
                p.Thread = thread;
            });

            return posts;
        }

        private static void Do<T>(IList<T> source, Action<T> action)
        {
            for (int i = 0; i < source.Count; i++)
                action(source[i]);
        }
    }
}
