using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vintage.NewBootOptions.Features;
public class RemoveRandom : CardAction
{
    public int count = 1;
    private static List<Type> BlacklistedCards /*{ get; set; }*/ = [
        typeof(BasicShieldColorless),
        typeof(DodgeColorless),
        typeof(CannonColorless),
        typeof(BasicSpacer),
        typeof(DroneshiftColorless),
        typeof(CorruptedCore)
    ];

    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        timer = 0.0;
        int deleteCardID = new int();

        bool cardWasDeleted = false;
        int i = 0;
        int j = 0;
        int breakCount = 100;

        while (!cardWasDeleted)
        {
            Random random = new Random();
            Card? card = s.deck.Shuffle(s.rngActions).FirstOrDefault();

            if (card != null)
            {
                if (!(BlacklistedCards.Contains(card.GetType())))
                {
                    deleteCardID = card.uuid;
                    cardWasDeleted = true;
                }
            }

            j++;

            if (j++ > breakCount) 
            {
                break;
            }

        }

        if (cardWasDeleted)
        {
            return new ShowCardsRemoved
            {
                cardId = deleteCardID
            };
        }

        return null;
    }



















    public override List<Tooltip> GetTooltips(State s)
    {
        return new List<Tooltip>
        {
            new TTGlossary("action.removeCard")
        };
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(StableSpr.icons_removeCard, 1, Colors.textMain);
    }
}