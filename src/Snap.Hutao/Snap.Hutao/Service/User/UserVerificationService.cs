// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.Web.Hoyolab.Passport;

namespace Snap.Hutao.Service.User;

[Service(ServiceLifetime.Transient, typeof(IUserVerificationService))]
internal sealed partial class UserVerificationService : IUserVerificationService
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial UserVerificationService(IServiceProvider serviceProvider);

    public async ValueTask<bool> TryVerifyAsync(IVerifyProvider provider, string? rawRisk, bool isOversea, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(rawRisk))
        {
            return false;
        }

        Risk? risk = JsonSerializer.Deserialize<Risk>(rawRisk, jsonOptions);
        ArgumentNullException.ThrowIfNull(risk?.VerifyString);

        RiskVerify? riskVerify = JsonSerializer.Deserialize<RiskVerify>(risk.VerifyString, jsonOptions);
        ArgumentNullException.ThrowIfNull(riskVerify);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            UserAccountVerificationDialog verificationDialog = await contentDialogFactory
                .CreateInstanceAsync<UserAccountVerificationDialog>(scope.ServiceProvider)
                .ConfigureAwait(false);

            if (await verificationDialog.TryValidateAsync(riskVerify.Ticket, isOversea).ConfigureAwait(false))
            {
                risk.VerifyString = default;
                provider.Verify = JsonSerializer.Serialize(risk); // DO NOT pass json options, newline-less string is required
                return true;
            }

            return false;
        }
    }
}