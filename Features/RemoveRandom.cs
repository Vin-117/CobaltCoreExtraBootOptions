using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vintage.NewBootOptions.Features;
public class RemoveRandom : CardAction
{
    //public Upgrade upgradePath;

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

        bool cardWasDeleted = false;
        int i = 0;
        int j = 0;
        int breakCount = 100;

        while (i < count)
        {
            Random random = new Random();
            Card? card = s.deck.Shuffle(s.rngActions).FirstOrDefault();



            if (card != null)
            {
                if (!(BlacklistedCards.Contains(card.GetType())))
                {
                    g.state.RemoveCardFromWhereverItIs(card.uuid);
                    cardWasDeleted = true;
                }
            }

            if (cardWasDeleted) 
            {
                i++;
                cardWasDeleted = false;
            }

            j++;

            if (j++ > breakCount) 
            {
                break;
            }

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