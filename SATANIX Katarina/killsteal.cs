using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace SupaKS
{
    internal class KillSteal
    {
        public static bool UseQ
        {
            get { return SATANIXKatarina.Program.KillStealMenu["kQ"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseW
        {
            get { return SATANIXKatarina.Program.KillStealMenu["kW"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseE
        {
            get { return SATANIXKatarina.Program.KillStealMenu["kE"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseR
        {
            get { return SATANIXKatarina.Program.KillStealMenu["kR"].Cast<CheckBox>().CurrentValue; }
        }

        public static void Execute()
        {

            if (Player.Instance.IsUnderTurret()) return;

            var killableEnemies =
                EntityManager.Heroes.Enemies.Where(
                    t =>
                        t.IsValidTarget() && !t.HasUndyingBuff() &&
                        SATANIXKatarina.Damage.CalculateDamage(t, UseQ, UseW, UseE, UseR) >= t.Health);
            var target = TargetSelector.GetTarget(killableEnemies, DamageType.Magical);

            if (target == null) return;

            if (UseQ &&
                target.Health <= SATANIXKatarina.Damage.CalculateDamage(target, true, false, false, false))
            {
                CastQ(target);
            }

            else if (UseW &&
                     target.Health <=
                     SATANIXKatarina.Damage.CalculateDamage(target, false, true, false, false))
            {
                CastW(target);
            }

            else if (UseE &&
                     target.Health <=
                     SATANIXKatarina.Damage.CalculateDamage(target, false, false, true, false))
            {
                CastE(target);
            }

            else if (target.Health <=
                     SATANIXKatarina.Damage.CalculateDamage(target, false, false, false, true))
            {
                CastR(target);
            }

        }

        private static void CastQ(AIHeroClient target)
        {
            if (!SATANIXKatarina.Program.Q.IsReady()) return;

            if (SATANIXKatarina.Program.Q.IsInRange(target))
            {
                SATANIXKatarina.Program.Q.Cast(target);
            }
        }


        private static void CastW(AIHeroClient target)
        {
            if (!SATANIXKatarina.Program.W.IsReady()) return;

            if (SATANIXKatarina.Program.W.IsInRange(target))
            {
                SATANIXKatarina.Program.W.Cast();
            }
        }

        private static void CastE(AIHeroClient target)
        {
            if (!SATANIXKatarina.Program.E.IsReady()) return;

            if (SATANIXKatarina.Program.E.IsInRange(target))
            {
                SATANIXKatarina.Program.E.Cast(target);
            }
        }


        private static void CastR(AIHeroClient target)
        {
            if (!SATANIXKatarina.Program.R.IsReady()) return;

            if (SATANIXKatarina.Program.E.IsInRange(target))
            {
                SATANIXKatarina.Program.R.Cast();
            }

        }
    }
}