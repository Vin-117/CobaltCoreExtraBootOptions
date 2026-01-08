using HarmonyLib;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Input;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using Vintage.NewBootOptions.Artifacts;

namespace Vintage.NewBootOptions.Patches;

[HarmonyPatch(typeof(Events))] // This makes sure PatchAll hits them
public static class EventsPatchesBootUpside
{
    private static ModEntry Instance => ModEntry.Instance;

    [HarmonyTranspiler]
    [HarmonyPatch("BootSequence")]
    private static IEnumerable<CodeInstruction> Events_BootSequenceUpside_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        // ReSharper disable PossibleMultipleEnumeration
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find([
                    ILMatches.Ldloc<List<Choice>>(originalMethod).GetLocalIndex(out var choicesLocalIndex).ExtractLabels(out var labels),
                    ILMatches.Ldloc<Rand>(originalMethod),
                    ILMatches.Call("Shuffle"),
                ])
                .Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                    new CodeInstruction(OpCodes.Ldloc, choicesLocalIndex.Value),
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Events_BootSequenceUpside_Transpiler_ModifyChoices))),
                ])
                .AllElements();
        }
        catch (Exception ex)
        {
            ModEntry.Instance.Logger!.LogError("Could not patch method {DeclaringType}::{Method} - {Mod} probably won't work.\nReason: {Exception}", originalMethod.DeclaringType, originalMethod, ModEntry.Instance.Name, ex);
            return instructions;
        }
        // ReSharper restore PossibleMultipleEnumeration
    }

    private static List<string> FullLocKey(List<string> locKey, string lineKey) => [
        ..locKey, lineKey
    ];

    private static void Events_BootSequenceUpside_Transpiler_ModifyChoices(State state, List<Choice> choices)
    {
        string key = ".zone_first"; // Same as vanilla key for these
        List<string> locKey = ["event", "BootSequence"];

        choices.Add(new BootUpsideMaxShield
        {
            label = ModEntry.Instance.Localizations.Localize(FullLocKey(locKey, "gainMaxShield")),
            key = key,
            actions = 
            [
                new AShipUpgrades 
                {
                    actions = 
                    [
                        new AShieldMax 
                        {
                            amount = 2,
                            targetPlayer = true
                        }
                    ]
                }
            ]
        });

        choices.Add(new BootUpsideUpgradedCommonCard
        {
            label = ModEntry.Instance.Localizations.Localize(FullLocKey(locKey, "gainUpgradedCommon")),
            key = key,
            actions = 
            [   
                (CardAction)new ACardOffering
                {
                    amount = 3,
                    rarityOverride = Rarity.common,
                    canSkip = false,
                    overrideUpgradeChances = true
                }
            ]
        });

        choices.Add(new BootUpsideRemoveAllArtifactsForBossArtifact
        {
            label = ModEntry.Instance.Localizations.Localize(FullLocKey(locKey, "gainBossArtifactForNodes")),
            key = key,
            actions =
            [
                (CardAction)new AAddArtifact
                {
                    artifact = new NewBootOptionsRemoveAllArtifacts()
                },
                (CardAction)new AArtifactOffering
                {
                    amount = 1,
                    canSkip = false,
                    limitPools = new List<ArtifactPool> { ArtifactPool.Boss }
                }

            ]
        });

    }

}