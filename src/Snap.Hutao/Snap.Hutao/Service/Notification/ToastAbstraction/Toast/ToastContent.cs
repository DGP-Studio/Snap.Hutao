// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;
using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;
using Windows.Data.Xml.Dom;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Base toast element, which contains at least a visual element.
/// </summary>
internal sealed class ToastContent : INotificationContent
{
    /// <summary>
    /// Initializes a new instance of toast content. You must then set the Visual property, which is required for a toast notification. You can optionally set Audio, Actions, and more.
    /// </summary>
    public ToastContent()
    {
    }

    /// <summary>
    /// The visual element is required.
    /// </summary>
    public ToastVisual? Visual { get; set; }

    /// <summary>
    /// Specify custom audio options.
    /// </summary>
    public ToastAudio? Audio { get; set; }

    /// <summary>
    /// Optionally create custom actions with buttons and inputs (using <see cref="ToastActionsCustom"/>) or optionally use the system-default snooze/dismiss controls (with <see cref="ToastActionsSnoozeAndDismiss"/>).
    /// </summary>
    public IToastActions? Actions { get; set; }

    /// <summary>
    /// Specify the scenario, to make the toast behave like an alarm, reminder, or more.
    /// </summary>
    public ToastScenario Scenario { get; set; }

    /// <summary>
    /// The amount of time the toast should display. You typically should use the Scenario attribute instead, which impacts how long a toast stays on screen.
    /// </summary>
    public ToastDuration Duration { get; set; }

    /// <summary>
    /// A string that is passed to the application when it is activated by the toast. The format and contents of this string are defined by the app for its own use. When the user taps or clicks the toast to launch its associated app, the launch string provides the context to the app that allows it to show the user a view relevant to the toast content, rather than launching in its default way.
    /// </summary>
    public string? Launch { get; set; }

    /// <summary>
    /// Specifies what activation type will be used when the user clicks the body of this toast.
    /// </summary>
    public ToastActivationType ActivationType { get; set; }

    /// <summary>
    /// Retrieves the notification XML content as a string, so that it can be sent with a HTTP POST in a push notification.
    /// </summary>
    /// <returns>The notification XML content as a string.</returns>
    public string GetContent()
    {
        return ConvertToElement().GetContent();
    }

    /// <summary>
    /// Retrieves the notification XML content as a WinRT XmlDocument, so that it can be used with a local toast notification's constructor on either <see cref="Windows.UI.Notifications.ToastNotification"/> or <see cref="Windows.UI.Notifications.ScheduledToastNotification"/>.
    /// </summary>
    /// <returns>The notification XML content as a WinRT XmlDocument.</returns>
    public XmlDocument GetXml()
    {
        XmlDocument doc = new();
        doc.LoadXml(GetContent());

        return doc;
    }

    internal ElementToast ConvertToElement()
    {
        ElementToast toast = new()
        {
            ActivationType = ActivationType,
            Duration = Duration,
            Launch = Launch,
            Scenario = Scenario
        };

        if (Visual is not null)
        {
            toast.Visual = Visual.ConvertToElement();
        }

        if (Audio is not null)
        {
            toast.Audio = Audio.ConvertToElement();
        }

        if (Actions is not null)
        {
            toast.Actions = ConvertToActionsElement(Actions);
        }

        return toast;
    }

    private static ElementToastActions ConvertToActionsElement(IToastActions actions)
    {
        return actions switch
        {
            ToastActionsCustom custom => custom.ConvertToElement(),
            ToastActionsSnoozeAndDismiss dismiss => dismiss.ConvertToElement(),
            _ => throw new NotImplementedException("Unknown actions type: " + actions.GetType())
        };
    }
}