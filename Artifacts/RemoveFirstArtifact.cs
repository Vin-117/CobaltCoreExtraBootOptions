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

public class NewBootOptionsRemoveFirstArtifact : Artifact, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NewBootOptionsRemoveFirstArtifact", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NewBootOptionsRemoveFirstArtifact", "desc"]).Localize,
            Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/RemoveFirstArtifact.png")).Sprite
        });

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(MapArtifact), nameof(MapArtifact.MakeRoute)),
            prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(MapArtifact_MakeRoute_Prefix))
        );
    }

    //public static int artifactRemovalCount = 1;

    private static bool MapArtifact_MakeRoute_Prefix(State s, ref Route __result)
    {
        if (s.EnumerateAllArtifacts().FirstOrDefault(a => a is NewBootOptionsRemoveFirstArtifact) is not { } artifact)
            return true;

        //if ((from r in s.EnumerateAllArtifacts()
        //     where r is NewBootOptionsRemoveAllArtifacts
        //     select r).ToList().Count > 0) 
        //    return true;


        __result = new ArtifactReward
        {
            artifacts = ArtifactReward.GetOffering(s, 0)
        };


        if (artifact is NewBootOptionsRemoveFirstArtifact)
        {
            s.rewardsQueue.Add(
                new ALoseArtifact
                {
                    artifactType = artifact.Key()
                });
        }

        return false;
    }

}