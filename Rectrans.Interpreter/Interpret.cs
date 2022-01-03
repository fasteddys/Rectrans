using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rectrans.Interpreter
{
    public static class Interpret
    {
        public static string WithGoogle(string text, string? tl, string? sl = null)
            => WithGoogleAsync(text, tl, sl).ConfigureAwait(false).GetAwaiter().GetResult();

        public static async Task<string> WithGoogleAsync(string text, string? tl, string? sl = null)
        {
            if (tl is null) throw new ArgumentNullException(tl, "传入的目标语言为 `NULL`.");
            
            var client = new HttpClient();
            var response = await client.GetAsync($"https://translate.googleapis.com/translate_a/single?client=gtx&dt=t&sl={sl ?? "auto"}&tl={tl}&q={text}");

            if (response.IsSuccessStatusCode)
            {
                return HandleResponse1(await response.Content.ReadAsStringAsync());
            }

            response = await client.GetAsync($"https://clients5.google.com/translate_a/t?client=dict-chrome-ex&sl={sl ?? "auto"}&tl={tl}&q={text}");

            if (response.IsSuccessStatusCode)
            {
                return HandleResponse2(await response.Content.ReadAsStringAsync());
            }
            throw new HttpRequestException();
        }

        private static string HandleResponse1(string text)
        {
            // var text1 = text.Substring(4, text.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
            var array = JArray.Parse(text).First!.Children().Select(x => x.First)!.Values<string>();
            return string.Join("", array);
        }

        private static string HandleResponse2(string text)
        {
            var response = JsonConvert.DeserializeObject<Response2>(text)!;
            return string.Join("", response.Sentences.Select(x => x.Trans));
        }
    }

    internal class Response2
    {
        public IEnumerable<Sentence> Sentences { get; set; } = null!;
    }

    internal abstract class Sentence
    {
        public string Trans { get; set; } = null!;
    }
}