﻿using PixiEditor.Helpers;
using PixiEditor.Helpers.Extensions;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using PixiEditor.Localization;

namespace PixiEditor.Models.DataHolders;

[DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
public record struct KeyCombination(Key Key, ModifierKeys Modifiers)
{
    public static KeyCombination None => new(Key.None, ModifierKeys.None);

    public override string ToString() => ToString(false);

    private string ToString(bool forceInvariant)
    {
        StringBuilder builder = new();

        foreach (var modifier in Modifiers.GetFlags().OrderByDescending(x => x != ModifierKeys.Alt))
        {
            if (modifier == ModifierKeys.None) continue;

            string key = modifier switch
            {
                ModifierKeys.Control => new LocalizedString("CTRL_KEY"),
                ModifierKeys.Shift => new LocalizedString("SHIFT_KEY"),
                ModifierKeys.Alt => new LocalizedString("ALT_KEY"),
                _ => modifier.ToString()
            };

            builder.Append($"{key}+");
        }

        if (Key != Key.None)
        {
            builder.Append(InputKeyHelpers.GetKeyboardKey(Key, forceInvariant));
        }

        builder.Append('‎'); // left-to-right marker ensures WPF does not reverse the string when using punctuations as key
        return builder.ToString();
    }

    private string GetDebuggerDisplay() => ToString(true);
}
