using Prism.Events;

namespace Rectrans.Events;

public class SettingEvent : PubSubEvent<SettingEvent>
{
    /// <summary>
    /// The language of source
    /// </summary>
    public string SourceLanguage { get; set; } = null!;

    /// <summary>
    /// The language of destination
    /// </summary>
    public string DestinationLanguage { get; set; } = null!;

    /// <summary>
    /// The interval of auto translation
    /// </summary>
    public int AutomaticTranslationInterval { get; set; }
}