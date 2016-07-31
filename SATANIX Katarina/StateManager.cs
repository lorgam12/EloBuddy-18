using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SATANIXKatarina
{
    internal class StateManager
    {
        public static Menu ComboMenu, HarassMenu, FarmMenu, JungleMenu;
        private static bool isChanneling;
        private static float rStart;
        private static Random rnd = new Random();
        private static bool value;
        private static int kills = 0;
        private static int assists = 0;
        private static bool gotkill = false;
        private static bool gotassist = false;
        private static float RavailableTime = 0;
        private static bool RInCooldown = true;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Init()
        {
            ComboMenu = Program.menu.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo Settings");

            ComboMenu.AddLabel("swaggy pentas incoming boyz");
            ComboMenu.Add("cQ", new CheckBox("Use Q", true));
            ComboMenu.Add("cW", new CheckBox("Use W", true));
            ComboMenu.Add("cE", new CheckBox("Use E", true));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("If teamfight mode is activated, the script will only combo when the target is killable \nin order to get resets");
            ComboMenu.AddSeparator();
            ComboMenu.Add("TFmode", new KeyBind("Enable teamfight mode", false, KeyBind.BindTypes.PressToggle, 'N'));
            //ComboMenu.Add("OnlyKillable", new CheckBox("Only combo if target is killable", false));
            ComboMenu.Add("TFmodeharass", new CheckBox("harass with Q while teamfight mode is on", true));
            ComboMenu.AddSeparator();
	    ComboMenu.Add("ComboLogic", new ComboBox(" Combo Logic ", 2, "Q>E>W>R", "E>Q>W>R", "Random/dynamic"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("cR", new CheckBox("Use Smart R", false));
            ComboMenu.AddLabel("Uncheck Smart R if you just want Regular Ult usage");

            HarassMenu = Program.menu.AddSubMenu("Harass Settings");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("hQ", new CheckBox("Use Q", true));
            HarassMenu.Add("hW", new CheckBox("Use W", false));
            HarassMenu.Add("autoharass", new CheckBox("Auto Harass with Q W?", false));

            FarmMenu = Program.menu.AddSubMenu("Farming Settings");
            FarmMenu.AddGroupLabel("Farm Settings");
            FarmMenu.Add("fQ", new CheckBox("Q to WaveClear"));
            FarmMenu.Add("fW", new CheckBox("W to WaveClear"));
            FarmMenu.AddGroupLabel("LastHit Settings");
            FarmMenu.Add("lQ", new CheckBox("Use Q to Last Hit"));
            FarmMenu.Add("lW", new CheckBox("Use W to Last Hit"));


            JungleMenu = Program.menu.AddSubMenu("Jungle Settings");
            JungleMenu.AddGroupLabel("Jungle Settings");
            JungleMenu.Add("jQ", new CheckBox("Use Q in Jungle"));
            JungleMenu.Add("jW", new CheckBox("Use W in Jungle"));

            Game.OnTick += Game_OnUpdate;
            Orbwalker.OnPreAttack += Orbwalker_PreAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Player.OnIssueOrder += Player_OnIssueOrder;
        }

        private static void Orbwalker_PreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target.IsMe)
            {
                args.Process = !_Player.HasBuff("KatarinaR");
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            SupaKS.KillSteal.Execute();
            if (HasRBuff())
            {
                Orbwalker.DisableMovement = true;
                Orbwalker.DisableAttacking = true;
            }
            else
            {
                Orbwalker.DisableMovement = false;
                Orbwalker.DisableAttacking = false;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Harass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)) LastHit();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) Jungle();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) WaveClear();
            if (HarassMenu["autoharass"].Cast<CheckBox>().CurrentValue)
            {
                AutoHarass();
            }

        }
        private static bool HasRBuff()
        {
            return _Player.HasBuff("KatarinaR") || Player.Instance.Spellbook.IsChanneling || _Player.HasBuff("katarinarsound");
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.R)
            {
                RInCooldown = true;

                if (RavailableTime < Game.Time)
                {
                    if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Cooldown < 10f && Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level == 1)
                        RavailableTime = Game.Time + 90f;
                    else if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Cooldown > 60f && Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level == 2)
                        RavailableTime = Game.Time + 60f;
                    else if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Cooldown > 45f && Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level == 3)
                        RavailableTime = Game.Time + 45f;
                    else
                        RavailableTime = Game.Time + Player.Instance.Spellbook.GetSpell(SpellSlot.R).Cooldown;
                }

                isChanneling = true;
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                EloBuddy.SDK.Core.DelayAction(MovementOn, 2500);
                isChanneling = false;
            }
        }

        public static void MovementOn()
        {
            Orbwalker.DisableAttacking = false;
            Orbwalker.DisableMovement = false;
        }

        private static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && Environment.TickCount < rStart + 300 && args.Order == GameObjectOrder.MoveTo)
            {
                args.Process = false;
            }
        }
        public static void LastHit()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(a => a.Distance(Player.Instance) < 1400).OrderBy(a => a.Health);
            var minion = minions.FirstOrDefault();
            if (minion == null || !minion.IsValidTarget()) return;
            if (FarmMenu["lQ"].Cast<CheckBox>().CurrentValue && (Damage.QDamage(minion) > minion.Health) && Program.Q.IsReady() && Program.Q.IsInRange(minion))
            {
                Program.Q.Cast(minion);
            }
            if (FarmMenu["lW"].Cast<CheckBox>().CurrentValue && (Damage.WDamage(minion) > minion.Health) && Program.W.IsReady() && Program.W.IsInRange(minion))
            {
                Program.W.Cast();
            }

        }

        public static void WaveClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(a => a.Distance(Player.Instance) < 1400).OrderBy(a => a.Health);
            var minion = minions.FirstOrDefault();
            if (minion == null) return;
            
            if (FarmMenu["fQ"].Cast<CheckBox>().CurrentValue && (Damage.QDamage(minion) > minion.Health) && Program.Q.IsReady() && Program.Q.IsInRange(minion))
            {
                Program.Q.Cast(minion);
            }
            if (FarmMenu["fW"].Cast<CheckBox>().CurrentValue && (Damage.WDamage(minion) > minion.Health) && Program.W.IsReady() && Program.W.IsInRange(minion))
            {
                Program.W.Cast();
            }
        }

        public static void Jungle()
        {
            var source = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(a => a.MaxHealth).FirstOrDefault(b => b.Distance(Player.Instance) < 1300);
            if (source == null || !source.IsValidTarget()) return;

            if (JungleMenu["jQ"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && source.Distance(_Player) < Program.Q.Range)
            {
                Program.Q.Cast(source);
            }
            if (JungleMenu["jW"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && source.Distance(_Player) < Program.W.Range)
            {
                Program.W.Cast();
            }
        }
	private static void Combo1() //QEW
	{
            var target = TargetSelector.GetTarget(Program.E.Range, EloBuddy.DamageType.Magical);
            if (!target.IsValidTarget(Program.E.Range))
                return;

            if (Program.Q.IsReady() && _Player.Distance(target.Position) <= Program.Q.Range)
            {
                Program.Q.Cast(target);
            }

            if (Program.E.IsReady() && _Player.Distance(target.Position) < Program.E.Range && !Program.Q.IsReady())
            {
                if (ComboMenu["cR"].Cast<CheckBox>().CurrentValue &&
                    _Player.CountEnemiesInRange(500) > 2 &&
                    (!Program.R.IsReady() || !(Program.R.State == SpellState.Surpressed && Program.R.Level > 0)))
                    return;

                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                rStart = Environment.TickCount;

                Program.E.Cast(target);
            }
	}
	private static void Combo2() //EQW
	{
            var target = TargetSelector.GetTarget(Program.E.Range, EloBuddy.DamageType.Magical);
            if (Program.E.IsReady() && _Player.Distance(target.Position) < Program.E.Range)
            {
                if (ComboMenu["cR"].Cast<CheckBox>().CurrentValue &&
                    _Player.CountEnemiesInRange(500) > 2 &&
                    (!Program.R.IsReady() || !(Program.R.State == SpellState.Surpressed && Program.R.Level > 0)))
                    return;


                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                rStart = Environment.TickCount;
                Program.E.Cast(target);
            }

            if (Program.Q.IsReady() && _Player.Distance(target.Position) <= Program.Q.Range)
            {
                Program.Q.Cast(target);
            }
	}

        private static void RandomCombo() //Random/dynamic Combo
        {
            value = rnd.Next(100) % 2 == 0;
            if (value)
                Combo1();
            else
                Combo2();
        }
        public static void Combo()
        {

            var target = TargetSelector.GetTarget(Program.E.Range, EloBuddy.DamageType.Magical);
            if (target.IsValidTarget())
            {
                if ((target.Health > DamageIndicator.SpellDamage.GetTotalDamage(target)) && ComboMenu["TFmode"].Cast<KeyBind>().CurrentValue)
                {
                    if (Program.Q.IsReady() && _Player.Distance(target.Position) <= Program.Q.Range && ComboMenu["TFmodeharass"].Cast<CheckBox>().CurrentValue)
                    {
                        Program.Q.Cast(target);
                    }
                    else if (!Orbwalker.DisableAttacking && !Orbwalker.DisableMovement)
                        return;
                }
                else
                {
                    if (ComboMenu["ComboLogic"].Cast<ComboBox>().CurrentValue == 0)
                    {
                        Combo1();
                    }
                    else if (ComboMenu["ComboLogic"].Cast<ComboBox>().CurrentValue == 1)
                    {
                        Combo2();
                    }
                    else if (ComboMenu["ComboLogic"].Cast<ComboBox>().CurrentValue == 2)
                    {
                        RandomCombo();
                    }

                    if (Program.W.IsReady() && _Player.Distance(target.Position) <= Program.W.Range && Program.Q.IsOnCooldown)
                    {
                        Program.W.Cast();
                    }

                    if (Program.R.IsReady() &&
                        _Player.CountEnemiesInRange(Program.R.Range) > 0)
                    {
                        if (!Program.Q.IsReady() && !Program.E.IsReady() && !Program.W.IsReady())
                        {
                            Orbwalker.DisableAttacking = true;
                            Orbwalker.DisableMovement = true;
                            Program.R.Cast();

                            rStart = Environment.TickCount;
                        }
                    }
                }
            }
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(Program.Q.Range, EloBuddy.DamageType.Magical);
            if (target.IsValidTarget())
            {
                if (HarassMenu["hQ"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && Program.Q.IsInRange(target))
                {
                    Program.Q.Cast(target);
                }
                if (HarassMenu["hW"].Cast<CheckBox>().CurrentValue && Program.W.IsInRange(target) && Program.W.IsReady())
                {

                    Program.W.Cast();

                }
            }
        }

        static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Program.Q.Range, EloBuddy.DamageType.Magical);
            if (target.IsValidTarget())
            {
                if (Program.Q.IsInRange(target))
                {
                    Program.Q.Cast(target);
                }
                if (Program.W.IsInRange(target))
                {
                    Program.W.Cast();
                }
            }
        }

        public static bool AddRdamage()
        {
            if (Player.Instance.ChampionsKilled > kills)
            {
                kills++;
                gotkill = true;
            }

            if (Player.Instance.Assists > assists)
            {
                assists++;
                gotassist = true;
            }

            if (gotassist || gotkill)
            {
                RavailableTime -= 15f;
                gotassist = false;
                gotkill = false;
            }
            if (RavailableTime > Game.Time)
                RInCooldown = true;
            else
                RInCooldown = false;

            return !RInCooldown;
        }
     }
}
