using EloBuddy;
using EloBuddy.SDK;

namespace SATANIXKatarina
{
    internal class Damage
    {

        public static float CalculateDamage(Obj_AI_Base target, bool q, bool w, bool e, bool r)
        {
            var totaldamage = 0f;

            if (target == null) return totaldamage;

            if (q && Program.Q.IsReady())
            {
                totaldamage += QDamage(target);
            }

            if (w && Program.W.IsReady())
            {
                totaldamage = WDamage(target);
            }

            if (e && Program.E.IsReady())
            {
                totaldamage += EDamage(target);
            }

            if (r && Program.R.IsReady())
            {
                totaldamage += RDamage(target);
            }

            return totaldamage;
        }

        public static float QDamage(Obj_AI_Base target)
        {
            return Player.Instance.GetSpellDamage(target, SpellSlot.Q, DamageLibrary.SpellStages.Default);
        }

        public static float WDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 0f, 40f, 75f, 110f, 145f, 180f }[SATANIXKatarina.Program.W.Level] + (Player.Instance.TotalMagicalDamage * 0.25f) + (Player.Instance.FlatPhysicalDamageMod * 0.6f));
        }


        public static float EDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 0, 40, 70, 100, 130, 160 }[SATANIXKatarina.Program.E.Level] + (Player.Instance.TotalMagicalDamage * 0.25f));
        }

        public static float RDamage(Obj_AI_Base target)
        {
            return Player.Instance.GetSpellDamage(target, SpellSlot.R, DamageLibrary.SpellStages.Default);
        }
    }
}