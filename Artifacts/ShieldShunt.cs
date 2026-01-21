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

public class NewBootOptionsShieldShunt : Artifact, IRegisterable
{

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                pools = [ArtifactPool.EventOnly],
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NewBootOptionsShieldShunt", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NewBootOptionsShieldShunt", "desc"]).Localize,
            Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/ShieldShunt.png")).Sprite
        });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn == 1)
        {
            combat.Queue(new AStatus
            {
                status = Status.evade,
                statusAmount = 1,
                targetPlayer = true,
                artifactPulse = Key()
            });
        }
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        List<Tooltip> list = new List<Tooltip>();
        list.Add(new TTGlossary("status.evade"));
        return list;
    }

}