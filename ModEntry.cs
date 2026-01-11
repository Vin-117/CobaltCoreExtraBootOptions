using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Shockah.CustomRunOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vintage.NewBootOptions.Artifacts;
using Vintage.NewBootOptions.Cards;
using Vintage.NewBootOptions.Patches;
using TheJazMaster.MoreDifficulties;

namespace Vintage.NewBootOptions;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null!;

    internal IMoreDifficultiesApi? MoreDifficultiesApi { get; private set; }

    public string Name { get; } = typeof(ModEntry).Namespace!;

    internal Settings Settings { get; private set; } = new();
    internal Harmony Harmony { get; }

	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    private static List<Type> NewBootOptionsEventArtifacts = [
        typeof(NewBootOptionsFTLCasings),
        typeof(NewBootOptionsShieldShunt),
        typeof(NewBootOptionsRemoveFirstArtifact),
        typeof(NewBootOptionsRemoveAllArtifacts)
    ];

	private static List<Type> NewBootOptionsCards = [
		typeof (NewBootOptionsFTLCasingCard),
        typeof (NewBootOptionsSystemFailure)
		];


	private static IEnumerable<Type> AllRegisterableTypes =
		NewBootOptionsEventArtifacts
		.Concat(NewBootOptionsCards);




    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/en.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);

        Harmony.PatchAll(typeof(ModEntry).Assembly);

        foreach (var type in AllRegisterableTypes)
            AccessTools.DeclaredMethod(type, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);

        Settings = helper.Storage.LoadJson<Settings>(helper.Storage.GetMainStorageFile("json"));

        helper.ModRegistry.AwaitApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties", api => MoreDifficultiesApi = api);

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceUpside(
            "gainTrueRandomUpgrades",
            () => "Upgrade 3 random cards to random types",
            choice => choice is BootUpsideRandomUpgrades
        ));

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceUpside(
            "gainBasics",
            () => "Gain 1 of 5 basic cards",
            choice => choice is BootUpsideGainBasics
        ));

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceUpside(
            "gainMaxShield",
            () => "Gain 2 <c=boldPink>max shield</c>",
            choice => choice is BootUpsideMaxShield
        ));

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceUpside(
            "gainBossArtifactForNodes",
            () => "Gain a boss arifact, remove all <c=artifact>artifact</c> node rewards",
            choice => choice is BootUpsideRemoveAllArtifactsForBossArtifact
        ));

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceUpside(
            "gainUpgradedCommon",
            () => "Gain 1 of 3 upgraded common cards",
            choice => choice is BootUpsideUpgradedCommonCard
        ));


        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceDownside(
            "LoseOneThirdHull",
            () => "<c=downside>Lose 2</c> <c=hull>hull</c>",
            choice => choice is BootDownsideTwoHull
        ));

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceDownside(
            "GainSafetyOverride",
            () => "<c=downside>Gain 1 non-temp <c=trash>Safety Override</c></c>",
            choice => choice is BootDownsideSafetyOverride
        ));

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceDownside(
            "GainSystemFailure",
            () => "<c=downside>Gain 1 <c=trash>System Failure</c></c>",
            choice => choice is BootDownsideSystemFailure
        ));

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceDownside(
            "GainFTLCanister",
            () => "<c=downside>Gain 1 <c=trash>FTL Canister</c> every zone</c>",
            choice => choice is BootdownsideFTLCanister
        ));

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceDownside(
            "GainShieldShunt",
            () => "<c=downside>Lose 1 <c=keyword>shield</c> on combat start</c>",
            choice => choice is BootdownsideShieldShunt
        ));

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceDownside(
            "RemoveAddCorrupted",
            () => "<c=downside>Remove a card, then gain a</c> <c=trash>Corrupted Core</c>",
            choice => choice is BootdownsideRemoveAddCorrupted
        ));

        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", api => api.RegisterBootSequenceDownside(
            "RemoveFirstArtifact",
            () => "<c=downside>The first <c=artifact>artifact</c> map node is empty</c>",
            choice => choice is BootdownsideRemoveFirstArtifact
        ));

    }
}