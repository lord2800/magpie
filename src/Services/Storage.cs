namespace Magpie.Services;

using Magpie.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;

public interface IStorage : IDisposable
{
    public void Initialize();

    public IEnumerable<string> GetLists(string filter);
    public ListRecord GetList(string name);
    public void SaveList(ListRecord list);
    public void DeleteList(string name);
}

public record FileSystemStorageOptions(string directoryName);

[Autowire(typeof(IStorage))]
public sealed class FileSystemStorage(IFileSystem fileSystem, FileSystemStorageOptions options, JsonSerializer serializer) : IStorage
{
    private readonly IDirectoryInfo directory = fileSystem.DirectoryInfo.New(options.directoryName);
    private readonly EnumerationOptions enumerationOptions = new();

    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
        // nothing to do
    }

    [Initializer]
    public void Initialize()
    {
        if (!directory.Exists) {
            directory.Create();
        }
        enumerationOptions.IgnoreInaccessible = true;
        enumerationOptions.RecurseSubdirectories = false;
        enumerationOptions.MatchCasing = MatchCasing.CaseInsensitive;
        enumerationOptions.MatchType = MatchType.Simple;
        enumerationOptions.ReturnSpecialDirectories = false;
    }

    private IFileInfo? GetListFile(string listName)
    {
        var files = directory.GetFiles($"{listName}.json");
        return files.Length switch {
            0 => null,
            1 => files[0],
            _ => throw new($"Too many list files found that match ${listName}"),
        };
    }

    public IEnumerable<string> GetLists(string filter)
    {
        var lists = new List<string>();
        if (string.IsNullOrWhiteSpace(filter)) {
            filter = "*";
        }

        foreach (var file in directory.EnumerateFiles($"{filter}.json", enumerationOptions)) {
            lists.Add(file.Name);
        }

        return lists;
    }

    public ListRecord GetList(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) {
            throw new("Must provide a list name");
        }
        var file = GetListFile(name) ?? throw new($"No list found with name {name}");
        var result = serializer.Deserialize<ListRecord>(new JsonTextReader(file.OpenText()));
        if (result is null) {
            throw new($"Unable to load list {name}");
        }
        return result;
    }

    public void SaveList(ListRecord list)
    {
        if (string.IsNullOrWhiteSpace(list.Name)) {
            throw new("Must provide a list name");
        }
        var file = GetListFile(list.Name);
        var stream = file switch {
            null => fileSystem.File.OpenWrite(Path.Join(directory.FullName, $"{list.Name}.json")),
            _ => file.OpenWrite(),
        };
        using var writer = new JsonTextWriter(new StreamWriter(stream));
        serializer.Serialize(writer, list);
    }

    public void DeleteList(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) {
            throw new("Must provide a list name");
        }
        var file = GetListFile(name);
        file?.Delete();
    }
}
