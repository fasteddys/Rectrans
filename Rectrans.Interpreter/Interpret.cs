using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rectrans.Interpreter
{
    public static class Interpret
    {
        public static string WithGoogle(string text, string toLanguage, string? fromLanguage = null)
            => WithGoogleAsync(text, toLanguage, fromLanguage).ConfigureAwait(false).GetAwaiter().GetResult();

        public static async Task<string> WithGoogleAsync(string text, string toLanguage, string? fromLanguage = null)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"https://translate.googleapis.com/translate_a/single?client=gtx&dt=t&sl={fromLanguage ?? "auto"}&tl={toLanguage}&q={text}");

            if (response.IsSuccessStatusCode)
            {
                return HandleResponse1(await response.Content.ReadAsStringAsync());
            }

            response = await client.GetAsync($"https://clients5.google.com/translate_a/t?client=dict-chrome-ex&sl={fromLanguage ?? "auto"}&tl={toLanguage}&q={text}");

            if (response.IsSuccessStatusCode)
            {
                return HandleResponse2(await response.Content.ReadAsStringAsync());
            }
            throw new HttpRequestException();
        }

        private static string HandleResponse1(string text)
        {
            var text1 = text.Substring(4, text.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
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

    internal class Sentence
    {
        public string Trans { get; set; } = null!;
    }
}