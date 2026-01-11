using System;
using System.Collections.Generic;
using System.Linq;

namespace Vintage.NewBootOptions.Features;
public class AUpgradeTrueRandom : CardAction
{
    //public Upgrade upgradePath;

    public int count = 1;

    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        timer = 0.0;

        List<int> list = new List<int>();
        for (int i = 0; i < count; i++)
        {

            Random random = new Random();
            bool random_bool = random.Next(0, 2) != 0;

            if (random_bool)
            {
                Card? card = s.deck.Where((Card card3) => card3.upgrade == Upgrade.None && card3.GetMeta().upgradesTo.Contains(Upgrade.A)).Shuffle(s.rngActions).FirstOrDefault();
                if (card != null)
                {
                    Card card2 = card;
                    card2.upgrade = Upgrade.A;
                    list.Add(card2.uuid);
                }
            }
            else 
            {
                Card? card = s.deck.Where((Card card3) => card3.upgrade == Upgrade.None && card3.GetMeta().upgradesTo.Contains(Upgrade.B)).Shuffle(s.rngActions).FirstOrDefault();
                if (card != null)
                {
                    Card card2 = card;
                    card2.upgrade = Upgrade.B;
                    list.Add(card2.uuid);
                }
            }
        }

        if (list.Count > 0)
        {
            return new ShowCards
            {
                messageKey = "showcards.upgraded",
                cardIds = list.ToList()
            };
        }

        return null;
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        List<Tooltip> list = new List<Tooltip>();
        list.Add(new TTGlossary("action.upgradeCardRandom", Upgrade.A));
        list.Add(new TTGlossary("action.upgradeCardRandom", Upgrade.B));
        return list;
    }

    public override Icon? GetIcon(State s)
    {
        //Enum.TryParse<Spr>("icons_upgradeCardRandom", out var Icon);
        
        return new Icon(StableSpr.icons_upgradeCardRandom, 1, Colors.textMain);
    }
}