using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System.Linq;
using SharpDX.Direct3D9;

namespace SATANIXKatarina
{
    internal class Program
    {
                //Spells
        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Targeted E;
        public static Spell.Active R;
        public static Menu menu, DrawingMenu, KillStealMenu;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static void Main(string[] args)
        {
            if (args != null)
            {
                try
                {
                    Loading.OnLoadingComplete += Load_OnLoad;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static void Load_OnLoad(EventArgs a)
        {

            if (Player.Instance.Hero != Champion.Katarina) return;

            menu = MainMenu.AddMenu("SATANIX Katarina", "swag");
            menu.AddGroupLabel("SATANIX Katarina");
            menu.AddLabel("Version: " + "0.9.0.0");
            menu.AddSeparator();
            menu.AddLabel("Stanix");

            DrawingMenu = menu.AddSubMenu("Drawing", "paintings");
            DrawingMenu.AddGroupLabel("Drawing Settings");
            DrawingMenu.Add("dQ", new CheckBox("Draw Q", false));
            DrawingMenu.Add("dW", new CheckBox("Draw W", false));
            DrawingMenu.Add("dE", new CheckBox("Draw E", true));
            DrawingMenu.Add("dR", new CheckBox("Draw R", false));
            DrawingMenu.Add("dTFmode", new CheckBox("Draw teamfight mode status", true));

            KillStealMenu = menu.AddSubMenu("KillSteal", "KataKs");
            KillStealMenu.AddGroupLabel("Killsteal Settings");
            KillStealMenu.Add("kQ", new CheckBox("KS Q", true));
            KillStealMenu.Add("kW", new CheckBox("KS W", true));
            KillStealMenu.Add("kE", new CheckBox("KS E", true));
            KillStealMenu.Add("kR", new CheckBox("KS R", false));


            Q = new Spell.Targeted(SpellSlot.Q, 675);

            W = new Spell.Active(SpellSlot.W, 375);

            E = new Spell.Targeted(SpellSlot.E, 700);

            R = new Spell.Active(SpellSlot.R, 550);

            Drawing.OnDraw += Drawing_OnDraw;
            StateManager.Init();
            WardJumper.Init();
            DamageIndicator.DamageIndicator.Initialize(DamageIndicator.SpellDamage.GetTotalDamage);


            Chat.Print("SATANIX katarina Loaded", System.Drawing.Color.LightBlue);
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawingMenu["dQ"].Cast<CheckBox>().CurrentValue)
            {
               Circle.Draw(Q.IsReady() ? Color.Green : Color.Red, Q.Range, Player.Instance.Position); 
            }
            if (DrawingMenu["dW"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(W.IsReady() ? Color.Green : Color.Red, W.Range, Player.Instance.Position);
            }
            if (DrawingMenu["dE"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(E.IsReady() ? Color.Green : Color.Red, E.Range, Player.Instance.Position);
            }
            if (DrawingMenu["dR"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(R.IsReady() ? Color.Green : Color.Red, R.Range, Player.Instance.Position);
            }
        }
    }
}