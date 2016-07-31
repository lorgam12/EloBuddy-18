using EloBuddy;
using EloBuddy.SDK;

namespace DamageIndicator
{
    public static class SpellDamage
    {
        public static float GetTotalDamage(AIHeroClient target)
        {
            // Auto attack
            var damage = Player.Instance.GetAutoAttackDamage(target);

            // Q
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.Q).IsReady)
            {
                damage += Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 0f, 75f, 115f, 155f, 195f, 235f }[SATANIXKatarina.Program.Q.Level] + (Player.Instance.TotalMagicalDamage * 0.65f));
            }

            // W
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.W).IsReady)
            {
                damage += Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 0f, 40f, 75f, 110f, 145f, 180f }[SATANIXKatarina.Program.W.Level] + (Player.Instance.TotalMagicalDamage * 0.25f) + (Player.Instance.FlatPhysicalDamageMod * 0.6f));
            }

            // E
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.E).IsReady)
            {
                damage += Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 0, 40, 70, 100, 130, 160 }[SATANIXKatarina.Program.E.Level] + (Player.Instance.TotalMagicalDamage * 0.25f));
            }
            
            // R
            if(Player.Instance.Spellbook.GetSpell(SpellSlot.R).IsReady)
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.R);
            else if (SATANIXKatarina.StateManager.AddRdamage())
            {
                damage += Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 0f, 350f, 550f, 750f }[SATANIXKatarina.Program.R.Level] + (Player.Instance.TotalMagicalDamage * 2.5f) + (Player.Instance.FlatPhysicalDamageMod * 3.75f)); 
            }
            return damage;
        }
    }
}