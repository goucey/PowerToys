// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Community.PowerToys.Run.Plugin.Everything.Exceptions;

using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.Everything.SearchHelper
{
    internal class EverythingApi
    {
        public enum StateCode
        {
            OK,
            MemoryError,
            IPCError,
            RegisterClassExError,
            CreateWindowError,
            CreateThreadError,
            InvalidIndexError,
            InvalidCallError,
        }

        public enum RequestFlag
        {
            FileName = 0x00000001,
            Path = 0x00000002,
            FullPathAndFileName = 0x00000004,
            Extension = 0x00000008,
            Size = 0x00000010,
            DateCreated = 0x00000020,
            DateModified = 0x00000040,
            DateAccessed = 0x00000080,
            Attributes = 0x00000100,
            FileListFileName = 0x00000200,
            RunCount = 0x00000400,
            DateRun = 0x00000800,
            DateRecentlyChanged = 0x00001000,
            HighlightedFileName = 0x00002000,
            HighlightedPath = 0x00004000,
            HighlightedFullPathAndFileName = 0x00008000,
        }

        /// <summary>
        /// Gets or sets a value indicating whether [match path].
        /// </summary>
        /// <value> <c> true </c> if [match path]; otherwise, <c> false </c>. </value>
        public bool MatchPath
        {
            get
            {
                return EverythingApiDllImport.Everything_GetMatchPath();
            }

            set
            {
                EverythingApiDllImport.Everything_SetMatchPath(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [match case].
        /// </summary>
        /// <value> <c> true </c> if [match case]; otherwise, <c> false </c>. </value>
        public bool MatchCase
        {
            get
            {
                return EverythingApiDllImport.Everything_GetMatchCase();
            }

            set
            {
                EverythingApiDllImport.Everything_SetMatchCase(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [match whole word].
        /// </summary>
        /// <value> <c> true </c> if [match whole word]; otherwise, <c> false </c>. </value>
        public bool MatchWholeWord
        {
            get
            {
                return EverythingApiDllImport.Everything_GetMatchWholeWord();
            }

            set
            {
                EverythingApiDllImport.Everything_SetMatchWholeWord(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable regex].
        /// </summary>
        /// <value> <c> true </c> if [enable regex]; otherwise, <c> false </c>. </value>
        public bool EnableRegex
        {
            get
            {
                return EverythingApiDllImport.Everything_GetRegex();
            }

            set
            {
                EverythingApiDllImport.Everything_SetRegex(value);
            }
        }

        /// <summary>
        /// Searches the specified key word and reset the everything API afterwards
        /// </summary>
        /// <param name="query">  The key word. </param>
        /// <param name="settings"> The max count. </param>
        /// <param name="macroEnabled">是否启用宏</param>
        /// <param name="token"> when cancelled the current search will stop and exit (and would not reset) </param>
        /// <returns> list rsult </returns>
        public List<SearchResult> Search(Query query,  EverythingSettings settings, bool macroEnabled, CancellationToken token)
        {
            var results = new List<SearchResult>();

            if (string.IsNullOrEmpty(query.Search))
            {
#pragma warning disable CA2208 // 正确实例化参数异常
                throw new ArgumentNullException(nameof(query.Search));
#pragma warning restore CA2208 // 正确实例化参数异常
            }

            string searchKeyword = SearchKeywordFormat(settings, query, out int maxCount, macroEnabled);

            if (maxCount < 0)
            {
#pragma warning disable CA2208 // 正确实例化参数异常
                throw new ArgumentOutOfRangeException(nameof(settings.MaxSearchCount));
#pragma warning restore CA2208 // 正确实例化参数异常
            }
#if DEBUG
            Log.Info($"Search:{searchKeyword}", this.GetType(), "Search");
#endif

            if (token.IsCancellationRequested)
            {
                return results;
            }

            // if (keyWord.StartsWith("@"))
            // {
            //     EverythingApiDllImport.Everything_SetRegex(true);
            //     //keyWord = keyWord.Substring(1);
            // }
            // else
            // {
            //     EverythingApiDllImport.Everything_SetRegex(false);
            // }
            // if (token.IsCancellationRequested) { return results; }
            EverythingApiDllImport.Everything_SetRequestFlags(RequestFlag.HighlightedFileName | RequestFlag.HighlightedFullPathAndFileName);
            if (token.IsCancellationRequested)
            {
                return results;
            }

            EverythingApiDllImport.Everything_SetOffset(0);
            if (token.IsCancellationRequested)
            {
                return results;
            }

            EverythingApiDllImport.Everything_SetMax(maxCount);
            if (token.IsCancellationRequested)
            {
                return results;
            }

            _ = EverythingApiDllImport.Everything_SetSearchW(searchKeyword);

            if (token.IsCancellationRequested)
            {
                return results;
            }

            if (!EverythingApiDllImport.Everything_QueryW(true))
            {
                CheckAndThrowExceptionOnError();
                return results;
            }

            if (token.IsCancellationRequested)
            {
                return results;
            }

            int count = EverythingApiDllImport.Everything_GetNumResults();

            for (int idx = 0; idx < count; ++idx)
            {
                if (token.IsCancellationRequested)
                {
                    return results;
                }

                // if (fileExtensions != null && fileExtensions.Any())
                // {
                //    string fileExtension = Marshal.PtrToStringUni(EverythingApiDllImport.Everything_GetResultExtension(idx));
                //    if (!fileExtensions.Any(item => item.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
                //        continue;
                // }
                string fileNameHighted = Marshal.PtrToStringUni(EverythingApiDllImport.Everything_GetResultHighlightedFileNameW(idx));
                string fullPathHighted = Marshal.PtrToStringUni(EverythingApiDllImport.Everything_GetResultHighlightedFullPathAndFileNameW(idx));

                if (fileNameHighted == null | fullPathHighted == null)
                {
                    CheckAndThrowExceptionOnError();
                }

                if (token.IsCancellationRequested)
                {
                    return results;
                }

                ConvertHighlightFormat(fileNameHighted, out List<int> fileNameHighlightData, out string fileName);
                if (token.IsCancellationRequested)
                {
                    return results;
                }

                ConvertHighlightFormat(fullPathHighted, out List<int> fullPathHighlightData, out string fullPath);

                var result = new SearchResult
                {
                    FileName = fileName,
                    FileNameHightData = fileNameHighlightData,
                    FullPath = fullPath,
                    FullPathHightData = fullPathHighlightData,
                };

                if (token.IsCancellationRequested)
                {
                    return results;
                }

                result.Type = EverythingApiDllImport.Everything_IsFolderResult(idx) ? ResultType.Folder : ResultType.File;

                results.Add(result);
            }

            return results;
        }

        // public void Load(string sdkPath)
        // {
        //    _ = LoadLibrary(sdkPath);
        // }
        private string SearchKeywordFormat(EverythingSettings settings, Query query, out int maxCount, bool macroEnabled)
        {
            string keyword = query.Search;
            if (!string.IsNullOrWhiteSpace(query.ActionKeyword) && query.Search.StartsWith(query.ActionKeyword, StringComparison.CurrentCultureIgnoreCase))
            {
                EverythingApiDllImport.Everything_SetRegex(true);

                // keyWord = keyWord.Substring(1);
                keyword = query.Search[1..];
            }
            else
            {
                EverythingApiDllImport.Everything_SetRegex(false);
            }

            Match match = Regex.Match(keyword, @"(count:(?<count>\d+))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            maxCount = match.Success ? int.Parse(match.Groups["count"].Value, CultureInfo.DefaultThreadCurrentCulture) : settings.MaxSearchCount;

            if (!macroEnabled)
            {
                return keyword;
            }

            if (!keyword.Contains(':'))
            {
                return keyword;
            }

            string[] keywordItems = keyword.Split(':');
            string separator = keywordItems?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(separator))
            {
                return keyword;
            }

            var macro = settings.Macros.FirstOrDefault(m => m.Prefix.Equals(separator, StringComparison.OrdinalIgnoreCase));
            return macro == null ? keyword : $"{macro.GetSearchFormat()} {string.Join(":", keywordItems.Skip(1))}";
        }

        private static void ConvertHighlightFormat(string contentHighlighted, out List<int> highlightData, out string fn)
        {
            highlightData = new List<int>();
            StringBuilder content = new StringBuilder();
            bool flag = false;
            char[] contentArray = contentHighlighted.ToCharArray();
            int count = 0;
            for (int i = 0; i < contentArray.Length; i++)
            {
                char current = contentHighlighted[i];
                if (current == '*')
                {
                    flag = !flag;
                    count = count + 1;
                }
                else
                {
                    if (flag)
                    {
                        highlightData.Add(i - count);
                    }

                    content.Append(current);
                }
            }

            fn = content.ToString();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int LoadLibrary(string name);

        private static void CheckAndThrowExceptionOnError()
        {
            switch (EverythingApiDllImport.Everything_GetLastError())
            {
                case StateCode.CreateThreadError:
                    throw new CreateThreadException();
                case StateCode.CreateWindowError:
                    throw new CreateWindowException();
                case StateCode.InvalidCallError:
                    throw new InvalidCallException();
                case StateCode.InvalidIndexError:
                    throw new InvalidIndexException();
                case StateCode.IPCError:
                    throw new IPCErrorException();
                case StateCode.MemoryError:
                    throw new MemoryErrorException();
                case StateCode.RegisterClassExError:
                    throw new RegisterClassExException();
            }
        }
    }
}
