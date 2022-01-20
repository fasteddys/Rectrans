using Prism.Events;

namespace Rectrans.Events;

public class OutputEvent : PubSubEvent<OutputEvent>
{
    /// <summary>
    /// The text of untranslated
    /// </summary>
    public string OriginalText { get; set; } = null!;

    /// <summary>
    /// The text of translation
    /// </summary>
    public string TranslationText { get; set; } = null!;

    /// <summary>
    /// The count of translation, if the <see cref="OriginalText"/> not changed
    /// count is 0
    /// </summary>
    public int Count { get; set; }
}