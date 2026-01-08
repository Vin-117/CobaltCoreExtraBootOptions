using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Vintage.NewBootOptions.Cards;


public class NewBootOptionsFTLCasingCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "NewBootOptionsFTLCasingCard", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/TrashBootCanister.png")).Sprite,
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
                        cost = 3,
                        singleUse = true,
                        description = string.Format(ModEntry.Instance.Localizations.Localize(["card", "NewBootOptionsFTLCasingCard", "desc"]))
                    };
                }
            default:
                {
                    return new CardData
                    {
                        cost = 3,
                        exhaust = true,
                        singleUse = true,
                        retain = true
                    };
                }
        }
    }
}