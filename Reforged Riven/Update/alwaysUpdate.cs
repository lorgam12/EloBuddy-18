﻿#region

using System;
using LeagueSharp;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using Reforged_Riven.Extras;
using Reforged_Riven.Menu;

#endregion

namespace Reforged_Riven.Update
{
    internal class PermaActive : Core
    {
       
        public static void Update(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }

            if (Environment.TickCount - Animation.LastQ >= 3670 - Game.Ping && Qstack != 1 &&
                !Player.InFountain() && MenuConfig.KeepQ && Player.HasBuff("RivenTriCleave"))
            {
                Spells.Q.Cast(Game.CursorPos);
            }

            Logic.ForceSkill();

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                {
                    Mode.Combo();
                    Mode.Burst();
                }
                    break;
                    case OrbwalkingMode.LaneClear:
                {
                    Mode.Lane();
                    Mode.Jungle();
                }

                    break;
                case OrbwalkingMode.None:
                {
                    Mode.Flee();
                    Mode.QMove();
                }
                    break;
                case OrbwalkingMode.Hybrid:
                    Mode.Harass();
                    break;
            }
        }
    }
}
