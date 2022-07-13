// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Community.PowerToys.Run.Plugin.Everything.SearchHelper
{
    public class SearchResult
    {
        public string FileName { get; set; }

        public List<int> FileNameHightData { get; set; }

        public string FullPath { get; set; }

        public List<int> FullPathHightData { get; set; }

        public ResultType Type { get; set; }
    }
}
