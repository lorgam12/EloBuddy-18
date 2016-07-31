using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using SharpDX.Direct3D9;
using Settings = DamageIndicator.Config.Modes.Draw;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;


namespace DamageIndicator
{
    public static class DamageIndicator
    {
        private const int BarWidth = 106;
        private const float LineThickness = 9.8f;

        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private static DamageToUnitDelegate DamageToUnit { get; set; }

        private static Font _Font, _Font2;

        public static void Initialize(DamageToUnitDelegate damageToUnit)
        {
            DamageToUnit = damageToUnit;
            Drawing.OnEndScene += OnEndScene;

            _Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                    Height = 18,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType,


                });

            _Font2 = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                    Height = 11,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType,

                });
        }

        private static void OnEndScene(EventArgs args)
        {
            if (SATANIXKatarina.Program.DrawingMenu["dTFmode"].Cast<CheckBox>().CurrentValue)
            {
                var pos = Drawing.WorldToScreen(Player.Instance.Position);
                if (SATANIXKatarina.StateManager.ComboMenu["TFmode"].Cast<KeyBind>().CurrentValue)
                    _Font2.DrawText(null, "TF mode enabled", (int)pos.X - 40, (int)pos.Y + 30, Color.White);
                else
                    _Font2.DrawText(null, "TF mode disabled", (int)pos.X - 40, (int)pos.Y + 30, Color.Orange);
            }
            if (Settings.DrawHealth || Settings.DrawPercent || Settings.DrawDmgToKill /*|| Settings.DrawStatistics*/)
            {
                foreach (var unit in EntityManager.Heroes.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    var damage = DamageToUnit(unit);

                    if (damage <= 0)
                    {
                        continue;
                    }

                    if (Settings.DrawHealth)
                    {
                        var damagePercentage = ((unit.TotalShieldHealth() - damage) > 0 ? (unit.TotalShieldHealth() - damage) : 0) /
                                               (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                        var currentHealthPercentage = unit.TotalShieldHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

                        var startPoint = new Vector2((int)(unit.HPBarPosition.X + damagePercentage * BarWidth), (int)unit.HPBarPosition.Y - 5 + 14);
                        var endPoint = new Vector2((int)(unit.HPBarPosition.X + currentHealthPercentage * BarWidth) + 1, (int)unit.HPBarPosition.Y - 5 + 14);

                        var colorH = System.Drawing.Color.FromArgb(Settings.HealthColor.A - 120, Settings.HealthColor.R,
                            Settings.HealthColor.G, Settings.HealthColor.B);

                        Drawing.DrawLine(startPoint, endPoint, LineThickness, colorH);
                    }
                    var color = new Color(Settings.HealthColor.R, Settings.HealthColor.G, Settings.HealthColor.B, Settings.HealthColor.A - 5);
                    if (Settings.DrawPercent)
                    {
                        _Font.DrawText(null, string.Concat(Math.Ceiling(damage / unit.TotalShieldHealth() * 100), "%"), (int)unit.HPBarPosition[0] + 102, (int)unit.HPBarPosition[1] + 29, color);
                    }
                    /*if (Settings.DrawStatistics)
                    {
                        _Font2.DrawText(null, "-" + Math.Round(SpellDamage.GetTotalDamage(unit)) + " / " + Math.Round((unit.Health - SpellDamage.GetTotalDamage(unit))), (int)unit.HPBarPosition[0], (int)unit.HPBarPosition[1] - 8, color);
                    }*/
                    if (Settings.DrawDmgToKill)
                    {
                        if (Math.Round(SpellDamage.GetTotalDamage(unit)) > Math.Round((unit.Health)))
                        {
                            int OverDmg = (int)(Math.Round(SpellDamage.GetTotalDamage(unit) - unit.Health));
                            _Font2.DrawText(null, OverDmg + " OVERDAMAGE ", (int)unit.HPBarPosition[0], (int)unit.HPBarPosition[1] - 8, Color.Red);
                        }
                        else if (Math.Round(SpellDamage.GetTotalDamage(unit)) < Math.Round((unit.Health)))
                        {
                            int RemainingHP = (int)(Math.Round(unit.Health) - SpellDamage.GetTotalDamage(unit));
                            _Font2.DrawText(null, RemainingHP + "HP after combo ", (int)unit.HPBarPosition[0], (int)unit.HPBarPosition[1] - 8, Color.White);
                        }
                    }
                }
            }
        }
    }
}