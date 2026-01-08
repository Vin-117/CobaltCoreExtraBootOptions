using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Vintage.NewBootOptions.Cards;


public class NewBootOptionsSystemFailure : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "NewBootOptionsSystemFailure", "name"]).Localize, 
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/sysfailedcard.png")).Sprite,
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
                        cost = 4,
                        singleUse = true,
                        //description = string.Format(ModEntry.Instance.Localizations.Localize(["card", "NewBootOptionsFTLCasingCard", "desc"]))
                    };
                }
            default:
                {
                    return new CardData
                    {
                        cost = 4,
                        exhaust = true,
                        singleUse = true,
                        retain = true
                    };
                }
        }
    }
}