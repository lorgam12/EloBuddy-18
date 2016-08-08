﻿using LeagueSharp.SDK;
using Reforged_Riven.Extras;

namespace Reforged_Riven.Update
{
    internal class Interrupt
    {
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            var target = args.Sender;

            if (!target.IsEnemy || !Spells.W.IsReady() || target.IsZombie || target.IsInvulnerable) return;

            if (Logic.InWRange(target))
            {
                Spells.W.Cast();
            }
        }
    }
}
