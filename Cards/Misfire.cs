using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Vintage.NewBootOptions.Cards;


public class NewBootOptionsMisfire : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Deck.trash,
                rarity = Rarity.common,
                dontOffer = true,
                //upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "NewBootOptionsMisfire", "name"]).Localize, 
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/misfirecard.png")).Sprite,
        });
    }

    public override CardData GetData(State state)
    {

        switch (this.upgrade) 
        {
            case Upgrade.None: 
                {
                    return new CardData 
                    {
                        cost = 0,
                        unplayable = true,
                        //description = string.Format(ModEntry.Instance.Localizations.Localize(["card", "NewBootOptionsMisfire", "desc"]))
                        description = ModEntry.Instance.Localizations.Localize(["card", "NewBootOptionsMisfire", "desc"], new { cnt = GetDmg(state, 1) })

                        //description = string.Format(ModEntry.Instance.Localizations.Localize(["card", "NewBootOptionsMisfire", "desc"], new { cnt = + GetDmg(state, 1) }))
                    };
                }
            default:
                {
                    return new CardData
                    {
                        cost = 0,
                        unplayable = true,
                        //description = string.Format(ModEntry.Instance.Localizations.Localize(["card", "NewBootOptionsMisfire", "desc"]))
                    };
                }
        }
    }

    public override void OnDraw(State s, Combat c)
    {
        c.Queue(new AAttack
        {
            targetPlayer = false,
            damage = GetDmg(s, 1)
        });
    }

}