using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace Rectrans.Infrastructure;

internal static class Google
{
    public static string Process(string text, string sl, string tl)
        => ProcessAsync(text, tl, sl).ConfigureAwait(false).GetAwaiter().GetResult();

    public static async Task<string> ProcessAsync(string text, string sl, string tl)
    {
        if (tl is null) throw new ArgumentNullException(tl, "传入的目标语言为 `NULL`.");

        var client = new HttpClient();
        var response = await client.GetAsync(
            $"https://translate.googleapis.com/translate_a/single?client=gtx&dt=t&sl={sl}&tl={tl}&q={text}");

        if (response.IsSuccessStatusCode)
        {
            return HandleResponse1(await response.Content.ReadAsStringAsync());
        }

        response = await client.GetAsync(
            $"https://clients5.google.com/translate_a/t?client=dict-chrome-ex&sl={sl}&tl={tl}&q={text}");

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

// ReSharper disable once ClassNeverInstantiated.Global
internal class Sentence
{
    public string Trans { get; set; } = null!;
}