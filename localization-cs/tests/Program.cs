// Regression checks for the unofficial Czech localization, 2026-09-29.
// Distributed under the upstream AGPL license; see the repository LICENSE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using UVtools.UI.Localization;

var tests = new Dictionary<string, string>
{
    ["_Edit print parameters"] = "_Upravit tiskové parametry",
    ["_Repair layers and issues"] = "_Opravit vrstvy a problémy",
    ["_Exposure time finder"] = "_Test doby expozice",
    ["_XYZ Accuracy"] = "_Přesnost XYZ",
    ["Layers: 203 @ 0.05mm"] = "Vrstvy: 203 @ 0.05mm",
    ["Pixels: 123\nVolume: 1.25mm³"] = "Pixely: 123\nObjem: 1.25mm³",
    ["Material: Test user resin 1.2"] = "Materiál: Test user resin 1.2",
    ["example.goo"] = "example.goo",
    ["1.25"] = "1.25",
    ["Welcome to UVtools v7.0.0!"] = "Vítejte v UVtools 7.0.0!"
};
foreach (var (english, expected) in tests)
{
    var actual = CzechLocalization.Translate(english);
    if (actual != expected) throw new Exception($"Failed: {english} => {actual}");
}
// Every mapped string must be stable when passed through the handler a second time.
var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(
    File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Languages/cs.json")))!;
foreach (var (english, translated) in dictionary)
{
    var again = CzechLocalization.Translate(translated);
    if (again != translated) throw new Exception($"Non-idempotent: {english} => {translated} => {again}");
}
CzechLocalization.Install();
var block = new TextBlock { Text = "_Edit print parameters" };
if (block.Text != "_Upravit tiskové parametry") throw new Exception("Initial visual translation failed");
block.Text = "Layers: 10 @ 0.05mm";
if (block.Text != "Vrstvy: 10 @ 0.05mm") throw new Exception("Live visual update failed");
Console.WriteLine($"PASS: {tests.Count} regression cases; {dictionary.Count} dictionary entries idempotent; live TextBlock updates.");
