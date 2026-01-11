using FMOD;
using JetBrains.Annotations;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vintage.NewBootOptions;
using Vintage.NewBootOptions.Cards;

namespace Vintage.NewBootOptions.Artifacts;

public class NewBootOptionsFTLCasings : Artifact, IRegisterable
{

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                pools = [ArtifactPool.EventOnly],
                unremovable = true
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NewBootOptionsFTLCasings", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NewBootOptionsFTLCasings", "desc"]).Localize,
            Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/OldFTLCasing.png")).Sprite
        });
    }

    public bool passedZoneOne = false;
    public bool passedZoneTwo = false;
    public bool passedZoneThree = false;
    //public bool



    public override List<Tooltip>? GetExtraTooltips()
    {
        return new List<Tooltip>
        {
            new TTCard
            {
                card = new NewBootOptionsFTLCasingCard()
                {
                    upgrade = Upgrade.None
                }
            }
        };
    }

    public override void OnCombatStart(State s, Combat c) 
    {

        if (s.map is MapFirst && !passedZoneOne) 
        {
            passedZoneOne = true;
            c.QueueImmediate(new AAddCard
            {
                card = new NewBootOptionsFTLCasingCard()
                {
                    upgrade = Upgrade.None
                },
                destination = CardDestination.Deck,
                amount = 1
            });
        }

        if (s.map is MapLawless && !passedZoneTwo) 
        {
            passedZoneTwo = true;
            c.QueueImmediate(new AAddCard
            {
                card = new NewBootOptionsFTLCasingCard()
                {
                    upgrade = Upgrade.None
                },
                destination = CardDestination.Deck,
                amount = 1
            });
        }

        if (s.map is MapThree && !passedZoneThree)
        {
            passedZoneThree = true;
            c.QueueImmediate(new AAddCard
            {
                card = new NewBootOptionsFTLCasingCard()
                {
                    upgrade = Upgrade.None
                },
                destination = CardDestination.Deck,
                amount = 1
            });
        }
    }

}