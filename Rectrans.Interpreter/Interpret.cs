using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rectrans.Interpreter
{
    public static class Interpret
    {
#if NET5_0_OR_GREATER
        public static string WithGoogle(string text, string toLanguage, string? fromLanguage = null)
#else
        public static string WithGoogle(string text, string toLanguage, string fromLanguage)
#endif
            => WithGoogleAsync(text, toLanguage, fromLanguage).ConfigureAwait(false).GetAwaiter().GetResult();


#if NET5_0_OR_GREATER
        public static async Task<string> WithGoogleAsync(string text, string toLanguage, string? fromLanguage = null)
        {
#else
        public static async Task<string> WithGoogleAsync(string text, string toLanguage, string fromLanguage)
        {   
#endif
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
#if NET5_0_OR_GREATER
            var array = JArray.Parse(text).First!.Children().Select(x => x.First)!.Values<string>();
#else
#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。
            var array = JArray.Parse(text).First.Children().Select(x => x.First).Values<string>();
#endif
            return string.Join("", array);
        }

        private static string HandleResponse2(string text)
        {
#if NET5_0_OR_GREATER
            var response = JsonConvert.DeserializeObject<Response2>(text)!;
#else
#pragma warning disable CS8602 // 解引用可能出现空引用。
            var response = JsonConvert.DeserializeObject<Response2>(text);
#endif
            return string.Join("", response.Sentences.Select(x => x.Trans));
        }
    }

    internal class Response2
    {
#if NET5_0_OR_GREATER
        public IEnumerable<Sentence> Sentences { get; set; } = null!;
#else
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public IEnumerable<Sentence> Sentences { get; set; }
#endif
    }

    internal class Sentence
    {
#if NET5_0_OR_GREATER
        public string Trans { get; set; } = null!;
#else
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public string Trans { get; set; }
#endif
    }
}