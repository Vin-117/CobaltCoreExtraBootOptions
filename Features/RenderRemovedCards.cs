using daisyowl.text;
using FSPRO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vintage.NewBootOptions.Features;

public class ShowCardsRemoved : Route, OnMouseDown, OnInputPhase
{
    public int cardId;

    public const double ANIMATION_DURATION = 1.0;

    public static Vec CARD_POS = G.screenSize / 2.0 - new Vec(58.0, 80.0) / 2.0;

    public double? deleteAnimationTimer;

    public override bool CanBePeeked()
    {
        return true;
    }

    public void FinallyReallyDelete(G g)
    {
        Analytics.Log(g.state, "deleteCard", new
        {
            card = g.state.deck.FirstOrDefault((Card c) => c.uuid == cardId)?.Key()
        });
        g.state.RemoveCardFromWhereverItIs(cardId);
    }

    public override void Render(G g)
    {
        g.Push(null, null, null, autoFocus: false, noHoverSound: false, gamepadUntargetable: false, ReticleMode.Quad, null, null, this);
        Card? card = g.state.FindCard(cardId);
        if (card != null)
        {
            string str = "This card will be removed."; //Loc.T("confirmCard.reallyRemoveCard", "This card will be removed.");
            if (deleteAnimationTimer == 0.0)
            {
                for (int i = 0; i < 150; i++)
                {
                    PFX.screenSpaceExplosion.Add(new Particle
                    {
                        pos = Mutil.RandBox01() * new Vec(58.0, 80.0) + CARD_POS + Mutil.RandVel() * 10.0,
                        vel = Mutil.RandVel() * 10.0,
                        lifetime = 0.5 + Mutil.NextRand() * 1.0,
                        size = 9.0 + Mutil.NextRand() * 9.0,
                        gravity = 0.0 - (0.25 + Mutil.NextRand() * 0.25)
                    });
                }
            }

            if (deleteAnimationTimer.HasValue)
            {
                deleteAnimationTimer += g.dt;
            }

            card.UpdateAnimation(g);
            Draw.Sprite(StableSpr.cockpit_deletionChamber, 0.0, 0.0);
            Draw.Fill(Colors.redd.gain(Mutil.Remap(-1.0, 1.0, 0.05, 0.1, Math.Sin(g.state.time * 4.0))), BlendMode.Add);
            Color? color = Colors.textBold;
            TAlign? align = TAlign.Center;
            Color? outline = Colors.black;
            Draw.Text(str, 240.0, 69.0, null, color, null, null, null, align, dontDraw: false, null, outline);
            if (!deleteAnimationTimer.HasValue)
            {
                Vec? posOverride = CARD_POS;
                State fakeState = DB.fakeState;
                UIKey? downHint = Enum.Parse<UK>("cardRemove_yes");
                card.Render(g, posOverride, fakeState, ignoreAnim: false, ignoreHover: false, hideFace: false, hilight: false, showRarity: false, autoFocus: true, null, null, null, null, null, null, null, null, downHint);
            }

            Vec localV2 = new Vec(210.0, 193.0);

            PFX.screenSpaceExplosion.Render(g.dt);
            OnMouseDown onMouseDown = this;

            UIKey key2 = Enum.Parse<UK>("cardRemove_yes");
            string text2 = Loc.T("confirmCard.remove", "REMOVE");
            Color? textColor = Colors.redd;
            onMouseDown = this;
            SharedArt.ButtonText(g, localV2, key2, text2, textColor, null, inactive: false, onMouseDown);
        }
        else
        {
            if (FeatureFlags.Debug)
            {
                throw new Exception("Invalid card id to delete!");
            }

            g.state.ChangeRoute(() => new MapRoute());
        }

        g.Pop();
    }

    public void OnMouseDown(G g, Box b)
    {
        if (b.key == Enum.Parse<UK>("cardRemove_no") && !deleteAnimationTimer.HasValue)
        {
            deleteAnimationTimer = 0.0;
            Audio.Play(Event.CardHandling);
        }

        if (b.key == Enum.Parse<UK>("cardRemove_yes") && !deleteAnimationTimer.HasValue)
        {
            deleteAnimationTimer = 0.0;
            Audio.Play(Event.CardHandling);
        }
    }

    public void OnInputPhase(G g, Box b)
    {
        if (deleteAnimationTimer >= 1.0)
        {
            FinallyReallyDelete(g);
            g.CloseRoute(this, CCResult.Confirm);
        }
        else if ((Input.GetGpDown(Btn.B)) && !deleteAnimationTimer.HasValue)
        {
            FinallyReallyDelete(g);
            g.CloseRoute(this, CCResult.Confirm);
        }
    }
}