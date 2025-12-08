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
    uint GetCurrentItemCount();
    uint GetCurrentItemQuantity();
    float GetPercentComplete();
    void OpenListEditor();
    void Start();
    void Stop();
    void Toggle(bool state);
}

[Autowire(typeof(IOverlayController))]
public class OverlayController : IOverlayController
{
    private readonly IListEditorController listEditor;
    private readonly CurrentList currentList;

    public OverlayController(IListEditorController listEditor, CurrentList currentList, IGlobalSettings settings)
    {
        this.listEditor = listEditor;
        this.currentList = currentList;

        settings.OverlayStateChanged += Toggle;
    }

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
    }

    public void Stop()
    {
    }
#pragma warning restore RCS1016

    public void OpenListEditor() => listEditor.Open();
}
