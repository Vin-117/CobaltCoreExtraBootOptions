using FMOD;
using HarmonyLib;
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

public class NewBootOptionsRemoveAllArtifacts : Artifact, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NewBootOptionsRemoveAllArtifacts", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NewBootOptionsRemoveAllArtifacts", "desc"]).Localize,
            Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/RemoveAllArtifacts.png")).Sprite
        });

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(MapArtifact), nameof(MapArtifact.MakeRoute)),
            prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(MapArtifact_MakeRouteAllArtifacts_Prefix))
        );
    }

    private static bool MapArtifact_MakeRouteAllArtifacts_Prefix(State s, ref Route __result)
    {
        if (s.EnumerateAllArtifacts().FirstOrDefault(a => a is NewBootOptionsRemoveAllArtifacts) is not { } artifact)
            return true;

        __result =
            new ArtifactReward
            {
                artifacts = ArtifactReward.GetOffering(s, 0)
            }; 
        artifact.Pulse();
        return false;
    }

}