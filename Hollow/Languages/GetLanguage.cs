﻿using System.Collections.Generic;

namespace Hollow.Languages;

public static class GetLanguage
{
    public static readonly Dictionary<string, string> LanguageList = new()
    {
        {"English", "en-US"},
        {"简体中文", "zh-CN"}
    };
}