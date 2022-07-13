// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Community.PowerToys.Run.Plugin.Everything
{
    public class ContextMenu
    {
        public string Name { get; set; }

        public string Command { get; set; }

        public string Argument { get; set; }

        public string ImagePath { get; set; }
    }
}
