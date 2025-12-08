namespace Magpie;

using Dalamud.Game;
using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

[ExcludeFromCodeCoverage] // for now, revisit this with a better l10n impl
[Autowire]
public class Strings
{
    private readonly IClientState clientState;
    private static readonly Dictionary<ClientLanguage, CultureInfo> supportedCultures = new() {
        { ClientLanguage.English, CultureInfo.CreateSpecificCulture("en-US") },
        { ClientLanguage.French, CultureInfo.CreateSpecificCulture("fr-FR") },
        { ClientLanguage.German, CultureInfo.CreateSpecificCulture("de-DE") },
        { ClientLanguage.Japanese, CultureInfo.CreateSpecificCulture("ja-JP") },
    };

    private readonly Dictionary<string, string> LanguageKey;

    public Strings(IClientState clientState)
    {
        this.clientState = clientState;
        // ignore this until translated
#pragma warning disable IDE0072
        LanguageKey = clientState.ClientLanguage switch {
#pragma warning restore IDE0072
            // ClientLanguage.Japanese => throw new System.NotImplementedException(),
            // ClientLanguage.German => throw new System.NotImplementedException(),
            // ClientLanguage.French => throw new System.NotImplementedException(),
            // default to english if not supported
            _ => new() {
                {"MainWindowHeaderText",      "Gathering Lists"},
                {"ConfigCogTooltip",          "Show/Hide Configuration"},
                {"SearchListsTextHint",       "Search lists"},
                {"ListNameHeader",            "List Name"},
                {"OptionsHeader",             "Options"},
                {"OverlayWindowName",         "Status"},
                {"CurrentGatheringText",      "Currently Gathering: {0} ({1:P1})"},
                {"CurrentItemText",           "Current Item: {0} ({1}/{2})"},
                {"EditListText",              "Edit List"},
                {"EmptyListEditorWindowName", "Create new list"},
                {"ListEditorWindowName",      "Editing {0}"},
                {"ListNameInputHint",         "List Name"},
                {"ItemTabLabel",              "List Items"},
                {"GatherablesTabLabel",       "Gatherables"},
                {"FromRecipeTabLabel",        "From Recipe"},
                {"FromNodeTabLabel",          "From Node"},
                {"ListSettingsWindowName",    "{0} Settings"},
                {"AbilityOptionsLabel",       "Ability options ({0})"},
                {"UseAbilityLabel",           "Use {0}"},
                {"GlobalSettingsWindowName",  "Settings"},
                {"PluginInstalledText",       "{0} installed"},
                {"EnableAutoRetainerText",    "Enable AutoRetainer"},
                {"MultiModeText",             "Multi Mode"},
                {"RepairAtNPCText",           "Repair at NPC"},
                {"ShowOverlayText",           "Show overlay"},
                {"QuantityPopupTitle",        "Enter quantity"},
                {"AcceptText",                "OK"},
                {"CancelText",                "Cancel"},
            },
        };
    }

    public CultureInfo CurrentCulture => supportedCultures[clientState.ClientLanguage];
    // ignore that this method is fully static, I want it to not be static
#pragma warning disable CA1822
    public string GetMainWindowName() => $"{Plugin.Name} v{Plugin.Version}";
#pragma warning restore CA1822

    #region Main window
    public string GetMainWindowHeaderText() => LanguageKey["MainWindowHeaderText"];
    public string GetConfigCogTooltip() => LanguageKey["ConfigCogTooltip"];
    public string GetSearchListsTextHint() => LanguageKey["SearchListsTextHint"];
    public string GetListNameHeader() => LanguageKey["ListNameHeader"];
    public string GetOptionsHeader() => LanguageKey["OptionsHeader"];
    #endregion

    #region Overlay window
    public string GetOverlayWindowName() => LanguageKey["OverlayWindowName"];
    public string GetCurrentGatheringText(string item, float complete) =>
        string.Format(LanguageKey["CurrentGatheringText"], item, complete);
    public string GetCurrentItemText(string item, uint count, uint quantity) =>
        string.Format(LanguageKey["CurrentItemText"], item, count, quantity);
    public string GetEditListText() => LanguageKey["EditListText"];
    #endregion

    #region List editor window
    public string GetEmptyListEditorWindowName() => LanguageKey["EmptyListEditorWindowName"];
    public string GetListEditorWindowName(string listName) =>
        string.Format(LanguageKey["ListEditorWindowName"], listName);
    public string GetListNameInputHint() => LanguageKey["ListNameInputHint"];
    public string GetItemTabLabel() => LanguageKey["ItemTabLabel"];
    public string GetGatherablesTabLabel() => LanguageKey["GatherablesTabLabel"];
    public string GetRecipeTabLabel() => LanguageKey["FromRecipeTabLabel"];
    public string GetNodeTabLabel() => LanguageKey["FromNodeTabLabel"];
    public string GetQuantityPopupTitle() => LanguageKey["QuantityPopupTitle"];
    public string GetAcceptText() => LanguageKey["AcceptText"];
    public string GetCancelText() => LanguageKey["CancelText"];
    #endregion

    #region List settings window
    public string GetListSettingsWindowName(string listName) =>
        string.Format(LanguageKey["ListSettingsWindowName"], listName);
    public string GetAbilityOptionsLabel(string job) =>
        string.Format(LanguageKey["AbilityOptionsLabel"], job);
    public string GetUseAbilityLabel(string name) =>
        string.Format(LanguageKey["UseAbilityLabel"], name);
    #endregion

    #region Global settings window
    public string GetGlobalSettingsWindowName() => LanguageKey["GlobalSettingsWindowName"];
    public string GetPluginInstalledText(string plugin) => string.Format(LanguageKey["PluginInstalledText"], plugin);
    public string GetEnableAutoRetainerText() => LanguageKey["EnableAutoRetainerText"];
    public string GetMultiModeText() => LanguageKey["MultiModeText"];
    public string GetRepairAtNPCText() => LanguageKey["RepairAtNPCText"];
    public string GetShowOverlayText() => LanguageKey["ShowOverlayText"];
    #endregion
}
