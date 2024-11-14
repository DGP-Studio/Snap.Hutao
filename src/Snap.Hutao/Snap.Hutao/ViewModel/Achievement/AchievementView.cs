// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.Achievement;

internal sealed partial class AchievementView : ObservableObject,
    IEntityAccessWithMetadata<Model.Entity.Achievement, Model.Metadata.Achievement.Achievement>,
    IAdvancedCollectionViewItem
{
    public const int FullProgressPlaceholder = int.MaxValue;

    public AchievementView(Model.Entity.Achievement entity, Model.Metadata.Achievement.Achievement inner)
    {
        Entity = entity;
        Inner = inner;

        IsChecked = entity.Status >= AchievementStatus.STATUS_FINISHED;
    }

    public Model.Entity.Achievement Entity { get; }

    public Model.Metadata.Achievement.Achievement Inner { get; }

    public uint Order
    {
        get => Inner.Order;
    }

    public bool IsChecked
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                (Entity.Status, Entity.Time) = value
                    ? (AchievementStatus.STATUS_REWARD_TAKEN, DateTimeOffset.UtcNow)
                    : (AchievementStatus.STATUS_INVALID, default);

                OnPropertyChanged(nameof(Time));
            }
        }
    }

    public string Time
    {
        get => $"{Entity.Time.ToLocalTime():yyyy.MM.dd HH:mm:ss}";
    }
}