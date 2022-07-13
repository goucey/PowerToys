// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.PowerToys.Run.Plugin.Everything
{
    public class MacroSettings
    {
        public string Prefix { get; set; }

        public string FileExtensions { get; set; }

        internal string GetSearchFormat()
        {
            var itemArr = FileExtensions.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries)
                 .Where(i => !string.IsNullOrWhiteSpace(i));
            return $"ext:{string.Join(";", itemArr)}";
        }
    }
}
