﻿using PixiEditor.Localization;

namespace PixiEditor.Models.Commands.Attributes.Commands;

internal partial class Command
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class GroupAttribute : Attribute
    {
        public string InternalName { get; }

        public LocalizedString DisplayName { get; }

        /// <summary>
        /// Groups all commands that start with the name <paramref name="internalName"/>
        /// </summary>
        public GroupAttribute(string internalName, string displayName)
        {
            InternalName = internalName;
            DisplayName = displayName;
        }
    }
}
