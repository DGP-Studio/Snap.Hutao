// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System.Numerics;
using Windows.Foundation;

namespace Snap.Hutao.Control.Layout;

internal sealed class DefaultItemCollectionTransitionProvider : ItemCollectionTransitionProvider
{
    private const double DefaultAnimationDurationInMs = 300.0;

    static DefaultItemCollectionTransitionProvider()
    {
        AnimationSlowdownFactor = 1.0;
    }

    public static double AnimationSlowdownFactor { get; set; }

    protected override bool ShouldAnimateCore(ItemCollectionTransition transition)
    {
        return true;
    }

    protected override void StartTransitions(IList<ItemCollectionTransition> transitions)
    {
        List<ItemCollectionTransition> addTransitions = new();
        List<ItemCollectionTransition> removeTransitions = new();
        List<ItemCollectionTransition> moveTransitions = new();

        foreach (ItemCollectionTransition transition in addTransitions)
        {
            switch (transition.Operation)
            {
                case ItemCollectionTransitionOperation.Add:
                    addTransitions.Add(transition);
                    break;
                case ItemCollectionTransitionOperation.Remove:
                    removeTransitions.Add(transition);
                    break;
                case ItemCollectionTransitionOperation.Move:
                    moveTransitions.Add(transition);
                    break;
            }
        }

        StartAddTransitions(addTransitions, removeTransitions.Count > 0, moveTransitions.Count > 0);
        StartRemoveTransitions(removeTransitions);
        StartMoveTransitions(moveTransitions, removeTransitions.Count > 0);
    }

    private static void StartAddTransitions(IList<ItemCollectionTransition> transitions, bool hasRemoveTransitions, bool hasMoveTransitions)
    {
        foreach (ItemCollectionTransition transition in transitions)
        {
            ItemCollectionTransitionProgress progress = transition.Start();
            Visual visual = ElementCompositionPreview.GetElementVisual(progress.Element);
            Compositor compositor = visual.Compositor;

            ScalarKeyFrameAnimation fadeInAnimation = compositor.CreateScalarKeyFrameAnimation();
            fadeInAnimation.InsertKeyFrame(0.0f, 0.0f);

            if (hasMoveTransitions && hasRemoveTransitions)
            {
                fadeInAnimation.InsertKeyFrame(0.66f, 0.0f);
            }
            else if (hasMoveTransitions || hasRemoveTransitions)
            {
                fadeInAnimation.InsertKeyFrame(0.5f, 0.0f);
            }

            fadeInAnimation.InsertKeyFrame(1.0f, 1.0f);
            fadeInAnimation.Duration = TimeSpan.FromMilliseconds(
                DefaultAnimationDurationInMs * ((hasRemoveTransitions ? 1 : 0) + (hasMoveTransitions ? 1 : 0) + 1) * AnimationSlowdownFactor);

            CompositionScopedBatch batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            visual.StartAnimation("Opacity", fadeInAnimation);
            batch.End();
            batch.Completed += (_, _) => progress.Complete();
        }
    }

    private static void StartRemoveTransitions(IList<ItemCollectionTransition> transitions)
    {
        foreach (ItemCollectionTransition transition in transitions)
        {
            ItemCollectionTransitionProgress progress = transition.Start();
            Visual visual = ElementCompositionPreview.GetElementVisual(progress.Element);
            Compositor compositor = visual.Compositor;

            ScalarKeyFrameAnimation fadeOutAnimation = compositor.CreateScalarKeyFrameAnimation();
            fadeOutAnimation.InsertExpressionKeyFrame(0.0f, "this.CurrentValue");
            fadeOutAnimation.InsertKeyFrame(1.0f, 0.0f);
            fadeOutAnimation.Duration = TimeSpan.FromMilliseconds(DefaultAnimationDurationInMs * AnimationSlowdownFactor);

            CompositionScopedBatch batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            visual.StartAnimation(nameof(Visual.Opacity), fadeOutAnimation);
            batch.End();
            batch.Completed += (_, _) =>
            {
                visual.Opacity = 1.0f;
                progress.Complete();
            };
        }
    }

    private static void StartMoveTransitions(IList<ItemCollectionTransition> transitions, bool hasRemoveAnimations)
    {
        foreach (ItemCollectionTransition transition in transitions)
        {
            ItemCollectionTransitionProgress progress = transition.Start();
            Visual visual = ElementCompositionPreview.GetElementVisual(progress.Element);
            Compositor compositor = visual.Compositor;
            CompositionScopedBatch batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            // Animate offset.
            if (transition.OldBounds.X != transition.NewBounds.X ||
                transition.OldBounds.Y != transition.NewBounds.Y)
            {
                AnimateOffset(visual, compositor, transition.OldBounds, transition.NewBounds, hasRemoveAnimations);
            }

            batch.End();
            batch.Completed += (_, _) => progress.Complete();
        }
    }

    private static void AnimateOffset(Visual visual, Compositor compositor, Rect oldBounds, Rect newBounds, bool hasRemoveAnimations)
    {
        Vector2KeyFrameAnimation offsetAnimation = compositor.CreateVector2KeyFrameAnimation();

        offsetAnimation.SetVector2Parameter("delta", new Vector2(
            (float)(oldBounds.X - newBounds.X),
            (float)(oldBounds.Y - newBounds.Y)));
        offsetAnimation.SetVector2Parameter("final", default);
        offsetAnimation.InsertExpressionKeyFrame(0.0f, "this.CurrentValue + delta");
        if (hasRemoveAnimations)
        {
            offsetAnimation.InsertExpressionKeyFrame(0.5f, "delta");
        }

        offsetAnimation.InsertExpressionKeyFrame(1.0f, "final");
        offsetAnimation.Duration = TimeSpan.FromMilliseconds(
            DefaultAnimationDurationInMs * ((hasRemoveAnimations ? 1 : 0) + 1) * AnimationSlowdownFactor);

        visual.StartAnimation("TransformMatrix._41_42", offsetAnimation);
    }
}