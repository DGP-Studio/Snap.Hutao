// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using QRCoder;

namespace Snap.Hutao.Factory.QuickResponse;

[Service(ServiceLifetime.Singleton, typeof(IQRCodeFactory))]
internal class QRCodeFactory : IQRCodeFactory
{
    public byte[] Create(string source)
    {
        using (QRCodeGenerator generator = new())
        {
            using (QRCodeData data = generator.CreateQrCode(source, QRCodeGenerator.ECCLevel.Q))
            {
                using (BitmapByteQRCode code = new(data))
                {
                    return code.GetGraphic(10);
                }
            }
        }
    }
}
