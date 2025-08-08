using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using MiraiPalette.Avalonia.ViewModels;

namespace MiraiPalette.Avalonia;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        ArgumentNullException.ThrowIfNull(param);
        var viewType = Type.GetType($"MiraiPalette.Avalonia.Views.{param.GetType().Name?.Replace("ViewModel", string.Empty)}");
        return App.Current?.Services.GetRequiredService(viewType!) is not Control control
            ? throw new InvalidOperationException($"No control found for type {viewType}.")
            : control;
    }

    public bool Match(object? data) => data is ViewModelBase;
}