namespace Magpie.Controller;

using Magpie.Model;
using Magpie.Services;
using System.Collections.Generic;

public interface IMainController
{
    void DeleteList(string list);
    IEnumerable<string> GetLists(string filter);
    void OpenImport();
    void OpenListEditor(string? list);
    void ToggleConfigWindow();
}

[Autowire(typeof(IMainController))]
public class MainController(
    IGlobalSettingsController globalSettings,
    IListEditorController listEditor,
    IListRepository repository,
    CurrentList currentList
) : IMainController
{
    public void ToggleConfigWindow() => globalSettings.Toggle();

    public void OpenListEditor(string? list)
    {
        currentList.List = (list is not null) ? repository.GetList(list) : GatheringList.Empty;
        listEditor.Open();
    }

    public void OpenImport()
    {
    }

    public IEnumerable<string> GetLists(string filter) => repository.GetLists(filter);
    public void DeleteList(string list) => repository.DeleteList(list);
}
