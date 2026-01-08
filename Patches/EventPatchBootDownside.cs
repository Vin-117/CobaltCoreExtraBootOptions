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
using Vintage.NewBootOptions.Cards;

namespace Vintage.NewBootOptions.Patches;

[HarmonyPatch(typeof(Events))] // This makes sure PatchAll hits them
public static class EventsPatchesBootDownside
{
    private static ModEntry Instance => ModEntry.Instance;

    [HarmonyTranspiler]
    [HarmonyPatch("BootSequenceDownside")]
    private static IEnumerable<CodeInstruction> Events_BootSequenceDownside_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        // ReSharper disable PossibleMultipleEnumeration
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find([
                    ILMatches.Ldloc<Rand>(originalMethod).ExtractLabels(out var labels),
                    ILMatches.Call("Shuffle"),

                ])
                .Insert(SequenceMatcherPastBoundsDirection.Before,
                SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
                    new CodeInstruction(OpCodes.Dup).WithLabels(labels),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Events_BootSequenceDownside_Transpiler_ModifyChoices))),
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

    private static void Events_BootSequenceDownside_Transpiler_ModifyChoices(List<Choice> choices, State state)
    {
        string key = "BootSequence"; // Same as vanilla key for these
        List<string> locKey = ["event", "BootSequence"];

        if (state.ship.hull > 2) 
        {
            choices.Add(new BootDownsideTwoHull
            {
                label = ModEntry.Instance.Localizations.Localize(FullLocKey(locKey, "LoseTwoHull")),
                key = key,
                actions =
                [
                    new AHurt
                    {
                        hurtAmount = 2,
                        cannotKillYou = false,
                        targetPlayer = true
                    }
                ]
            });
        };

        choices.Add(new BootDownsideSafetyOverride
        {
            label = ModEntry.Instance.Localizations.Localize(FullLocKey(locKey, "GainSafetyOverride")),
            key = key,
            actions =
            [
                (CardAction)new AAddCard
                {
                    card = new TrashAutoShoot()
                    {
                        temporaryOverride = false
                    },
                    callItTheDeckNotTheDrawPile = true,
                },
            ]
        });

        choices.Add(new BootDownsideSystemFailure
        {
            label = ModEntry.Instance.Localizations.Localize(FullLocKey(locKey, "GainSystemFailure")),
            key = key,
            actions =
            [
                (CardAction)new AAddCard
                {
                    amount = 1,
                    card = new NewBootOptionsSystemFailure(),
                    callItTheDeckNotTheDrawPile = true,
                },
            ]
        });

        choices.Add(new BootdownsideFTLCanister
        {
            label = ModEntry.Instance.Localizations.Localize(FullLocKey(locKey, "GainFTLCanister")),
            key = key,
            actions =
            [
                (CardAction)new AAddArtifact
                {
                    artifact = new NewBootOptionsFTLCasings()
                }
            ]
        });

        choices.Add(new BootdownsideShieldShunt
        {
            label = ModEntry.Instance.Localizations.Localize(FullLocKey(locKey, "GainShieldShunt")),
            key = key,
            actions =
            [
                (CardAction)new AAddArtifact
                {
                    artifact = new NewBootOptionsShieldShunt()
                }
            ]
        });

        choices.Add(new BootdownsideRemoveAddCorrupted
        {
            label = ModEntry.Instance.Localizations.Localize(FullLocKey(locKey, "RemoveAddCorrupted")),
            key = key,
            actions =
            [
                (CardAction)new ARemoveCard(),
                (CardAction)new AAddCard
                {
                    card = new CorruptedCore()
                    {
                    },
                    callItTheDeckNotTheDrawPile = true,
                }
            ]
        });

        choices.Add(new BootdownsideRemoveFirstArtifact
        {
            label = ModEntry.Instance.Localizations.Localize(FullLocKey(locKey, "RemoveFirstArtifact")),
            key = key,
            actions =
            [
                (CardAction)new AAddArtifact
                {
                    artifact = new NewBootOptionsRemoveFirstArtifact()
                }
            ]
        });


    }
}