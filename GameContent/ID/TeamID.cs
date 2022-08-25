﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanksRebirth.Internals.Common.Framework.Collections;

namespace TanksRebirth.GameContent.ID
{
    /// <summary>These should all be ACTUAL colors, otherwise exceptions will happen.</summary>
    public sealed class TeamID
    {
        public const int NoTeam = 0;
        public const int Red = 1;
        public const int Blue = 2;
        public const int Green = 3;
        public const int Yellow = 4;
        public const int Purple = 5;
        public const int Orange = 6;
        public const int Cyan = 7;
        public const int Magenta = 8;
        public static ReflectionDictionary<TeamID, int> Collection = new(MemberType.Fields);
    }
}