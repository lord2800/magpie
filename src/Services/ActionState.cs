namespace Magpie.Services;

using ActionSheet = Lumina.Excel.Sheets.Action;
using Dalamud.Plugin.Services;
using Magpie.Data;
using System.Linq;
using TraitSheet = Lumina.Excel.Sheets.Trait;
using System.Collections.Generic;

public interface IActionState
{
    bool IsAvailable(in TraitSheet trait);
    bool IsAvailable(in ActionSheet action);
    bool IsAvailable(GatheringAbility ability);
}

[Autowire(typeof(IActionState))]
public class ActionState(
#pragma warning disable Dalamud001
    IUnlockState unlock,
#pragma warning restore Dalamud001
    IReadOnlyCollection<ActionSheet> actionSheet
) : IActionState
{
    public bool IsAvailable(in TraitSheet trait) => unlock.IsTraitUnlocked(trait);
    public bool IsAvailable(in ActionSheet action) => unlock.IsActionUnlocked(action);

    public bool IsAvailable(GatheringAbility ability)
        => IsAvailable((from _ in actionSheet where _.RowId == ability.Id select _).FirstOrDefault());
}
