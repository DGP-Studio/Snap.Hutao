// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Model;

/// <summary>
/// 可选择的对象
/// 默认为选中状态
/// </summary>
/// <typeparam name="T">值的类型</typeparam>
public class Selectable<T> : ObservableObject
    where T : class
{
    private readonly Action? selectedChanged;

    private bool isSelected = true;
    private T value;

    /// <summary>
    /// 构造一个新的可选择的对象
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="onSelectedChanged">选中的值发生变化时调用</param>
    public Selectable(T value, Action? onSelectedChanged = null)
    {
        this.value = value;
        selectedChanged = onSelectedChanged;
    }

    /// <summary>
    /// 指示当前对象是否选中
    /// </summary>
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            SetProperty(ref isSelected, value);
            selectedChanged?.Invoke();
        }
    }

    /// <summary>
    /// 存放的对象
    /// </summary>
    public T Value { get => value; set => SetProperty(ref this.value, value); }
}