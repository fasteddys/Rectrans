using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Rectrans.Model
{
    internal class Language
    {
        public string Name { get; set; } = null!;
        public string Abrr { get; set; } = null!;
        public string Tesseract { get; set; } = null!;
        public string Description { get; set; } = null!;

        private static readonly IEnumerable<Language>? SourceCollection = JsonConvert.DeserializeObject<IEnumerable<Language>>
            (File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Model/Json/languages.json")));

        public static string FindAbrr(string name)
        {
            return SourceCollection!.First(x => x.Name == name).Abrr;
        }

        public static string FindTesseract(string name)
        {
            return SourceCollection!.First(x => x.Name == name).Tesseract;
        }
    }
}
