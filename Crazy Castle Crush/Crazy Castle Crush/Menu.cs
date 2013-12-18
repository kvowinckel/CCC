using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.ScreenManagement.BaseScreens;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.Events;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NOVA.Scenery;
using NOVA.UI;
using NOVA.ScreenManagement;
using NOVA.Physics;
using NOVA.Utilities;
using NOVA;
using NOVA.Graphics;


namespace Crazy_Castle_Crush
{
    public class Menu : MenuScreen
    {
        public  Menu()
            : base("Menu")    //Titel des Menus wird definiert
        {
            //Einträge erzeugen
            MenuEntry playEntry = new MenuEntry("Play");
            MenuEntry optionsEntry = new MenuEntry("Options");

            //Delegates den Events zuweisen, wenn die Einträge ausgewählt werden
            playEntry.Selected += PlaySelected;
            optionsEntry.Selected += OptionsSelected;

            //Einträge zum Menu hinzufügen
            MenuEntries.Add(playEntry);
            MenuEntries.Add(optionsEntry);
        }

        void PlaySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(Core.ScreenManager, true, e.PlayerIndex, new Gamestart()); //erstes Level wird geladen
        }

        void OptionsSelected(object sender, PlayerIndexEventArgs e)
        {
            //LoadingScreen.Load(Core.ScreenManager, true, e.PlayerIndex, new OptionsMenu()); //Optionsmenu wird geladen
        }
    }
}
