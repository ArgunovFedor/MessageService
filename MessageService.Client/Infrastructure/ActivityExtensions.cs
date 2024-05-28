using System.Diagnostics;

namespace MessageService.Client.Infrastructure;

public static class ActivityExtensions
{
    /// <summary>
    /// Устанавливает аттрибут текущему Span'у и всем нижестоящим.
    /// Аттрибуты передаются между микросервисами общим контекстом (через заголовки).
    /// </summary>
    /// <param name="activity">Расширение для System.Diagnostics.Activity</param>
    /// <param name="key">Ключ аттрибута (Если такой ключ ранее был установлен, его значение будет перезаписано новым)</param>
    /// <param name="value">Значение аттрибута</param>
    /// <remarks>
    /// В общем контексте стоит использовать только важную информацию, например идентификаторы пользователя, процесса,
    /// номера заявок, и.т.д, чтобы не нагружать сборщик и не усложнять процесс трассировки
    /// </remarks>
    public static Activity SetSharedTag(this Activity activity, string key, string value)
    {
        activity?.SetTag(key, value);
        activity?.SetBaggage(key, value);
        return activity;
    }

    /// <summary>
    /// Устанавливает аттрибуты текущему Span'у и всем нижестоящим.
    /// Аттрибуты передаются между микросервисами общим контекстом (через заголовки).
    /// </summary>
    /// <param name="activity">Расширение для System.Diagnostics.Activity</param>
    /// <param name="tags">Словарь ключей->значений тегов (Если какой то из ключей ранее был установлен, его значение будет перезаписано новым)</param>
    /// <remarks>
    /// В общем контексте стоит использовать только важную информацию, например идентификаторы пользователя, процесса,
    /// номера заявок, и.т.д, чтобы не нагружать сборщик и не усложнять процесс трассировки
    /// </remarks>
    public static Activity SetSharedTags(this Activity activity, IDictionary<string, string> tags)
    {
        foreach (var (key, value) in tags)
        {
            activity.SetSharedTag(key, value);
        }

        return activity;
    }

    /// <summary>
    /// Позволяет получить значение аттрибута из общего контекста
    /// </summary>
    /// <param name="activity">Расширение для System.Diagnostics.Activity</param>
    /// <param name="key">Ключ аттрибута </param>
    /// <returns></returns>
    public static string GetSharedTag(this Activity activity, string key)
    {
        return activity?.GetBaggageItem(key);
    }
}