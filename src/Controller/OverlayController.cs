namespace Magpie.Controller;

using Magpie.Config;
using Magpie.Model;
using Magpie.Services;
using System;

public interface IOverlayController
{
    IGatheringList List { get; }

    event Action<bool>? ToggleWindow;

    string GetCurrentItem();
    uint GetCurrentItemCollected();
    uint GetCurrentItemAmount();
    float GetPercentComplete();
    void OpenListEditor();
    void Start();
    void Stop();
    void Toggle(bool state);
}

[Autowire(typeof(IOverlayController))]
public class OverlayController(
    IListEditorController listEditor,
    CurrentList currentList,
    IGlobalSettings settings,
    IOrchestrator orchestrator
) : IOverlayController
{
    [Initializer]
    public void Initialize() => settings.OverlayStateChanged += Toggle;

    public IGatheringList List { get => currentList.List; }
    public event Action<bool>? ToggleWindow;

    public void Toggle(bool state) => ToggleWindow?.Invoke(state);

#pragma warning disable RCS1016
    public float GetPercentComplete()
    {
        return 0.735f;
    }

    public string GetCurrentItem()
    {
        return "Rhodium Ore";
    }

    public uint GetCurrentItemCount()
    {
        return 2;
    }

    public uint GetCurrentItemQuantity()
    {
        return 25;
    }

    public void Start()
    {
        // TODO
    }

    public void Stop()
    {
        // TODO
    }
#pragma warning restore RCS1016

    public void OpenListEditor() => listEditor.Open();
}
