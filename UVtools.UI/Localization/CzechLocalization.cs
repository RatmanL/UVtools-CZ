// Unofficial Czech presentation-layer modification, 2026-09-29.
// Distributed under the upstream AGPL license; see the repository LICENSE.
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;

namespace UVtools.UI.Localization;

/// <summary>
/// Translates presentation text only. Model values, enum names, numeric culture,
/// file metadata and serialization remain in their original form.
/// The external dictionary can be edited without rebuilding the application.
/// </summary>
internal static partial class CzechLocalization
{
    private static readonly FrozenDictionary<string, string> Translations = Load();
    private static bool _installed;

    private static FrozenDictionary<string, string> Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Languages", "cs.json");
        var entries = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path))
            ?? throw new InvalidDataException("Czech translation dictionary is empty.");
        return entries.ToFrozenDictionary(StringComparer.Ordinal);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();

    [GeneratedRegex(@"(?m)^(Layers|Bottom layers|Transition layers|Exposure|Lift|Retract|Light-off|Wait time|Print time|Used material|Material cost|Material|Machine|Properties|Groups|Lines|Chars|Operations|Pixels|Volume|Bounds|Masks|Zoom|Profiles|Region|Elapsed Time|Materials|On time|Off time):")]
    private static partial Regex StatusLabels();

    internal static string Translate(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        if (Translations.TryGetValue(text, out var translated)) return translated;
        var key = text.Trim();
        if (key.AsSpan().IndexOfAny('\n', '\r', '\t') >= 0 || key.Contains("  ", StringComparison.Ordinal))
            key = Whitespace().Replace(key, " ");
        if (Translations.TryGetValue(key, out translated)) return translated;
        // Dynamic menus add the access-key marker after obtaining the operation title.
        if (key.Length > 1 && key[0] == '_' && Translations.TryGetValue(key[1..], out translated))
            return "_" + translated.TrimStart('_');
        // Only translate known labels; numbers and user values after the colon stay intact.
        if (text.Contains(':'))
        {
            return StatusLabels().Replace(text, match =>
                Translations.TryGetValue(match.Value, out var label) ? label : match.Value);
        }
        return text;
    }

    internal static void Install()
    {
        if (_installed) return;
        _installed = true;
        // Update rendered text, not Content/SelectedItem or editable TextBox values.
        // SetCurrentValue preserves the original binding and subsequent live updates.
        TextBlock.TextProperty.Changed.AddClassHandler<TextBlock>((control, _) =>
            Update(control, TextBlock.TextProperty));
        Run.TextProperty.Changed.AddClassHandler<Run>((control, _) =>
            Update(control, Run.TextProperty));
        Window.TitleProperty.Changed.AddClassHandler<Window>((control, _) =>
            Update(control, Window.TitleProperty));
    }

    private static void Update(AvaloniaObject control, StyledProperty<string?> property)
    {
        var original = control.GetValue(property);
        if (original is null) return;
        var translated = Translate(original);
        if (!string.Equals(original, translated, StringComparison.Ordinal))
            control.SetCurrentValue(property, translated);
    }
}
