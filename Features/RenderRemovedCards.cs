using daisyowl.text;
using FSPRO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vintage.NewBootOptions.Features;


public class ShowCardsRemoved : Route, OnMouseDown
{
    public List<int> cardIds = new List<int>();

    public void OnMouseDown(G g, Box b)
    {
        
        //if (b.key == UK.shipUpgrades_continue)
        if (b.key == Enum.Parse<UK>("shipUpgrades_continue"))
        {
            Audio.Play(Event.Click);
            g.CloseRoute(this);
            foreach (int item2 in cardIds)
            {
                g.state.RemoveCardFromWhereverItIs(item2);
            }
        }
    }

    public override void Render(G g)
    {
        List<Card> list = (from cid in cardIds
                           select g.state.FindCard(cid) into c
                           where c != null
                           select c).ToList();
        CardUtils.FanOut(list, new Vec(240.0, 90.0));
        foreach (Card item in list)
        {
            item.UpdateAnimation(g);
        }

        Draw.Sprite(StableSpr.cockpit_deletionChamber, 0.0, 0.0);
        Draw.Fill(Colors.redd.gain(Mutil.Remap(-1.0, 1.0, 0.05, 0.1, Math.Sin(g.state.time * 4.0))), BlendMode.Add);
        string str = "Removed!";
        Color? color = Colors.textBold;
        TAlign? align = TAlign.Center;
        Color? outline = Colors.black;
        Draw.Text(str, 240.0, 69.0, null, color, null, null, null, align, dontDraw: false, null, outline);
        SharedArt.ButtonText(g, new Vec(210.0, 193.0), Enum.Parse<UK>("shipUpgrades_continue"), Loc.T("uiShared.btnContinue"), null, null, inactive: false, this, null, null, null, null, autoFocus: true);
        foreach (Card item2 in list)
        {
            G g2 = g;
            State fakeState = DB.fakeState;
            item2.Render(g2, null, fakeState);
        }
    }
}