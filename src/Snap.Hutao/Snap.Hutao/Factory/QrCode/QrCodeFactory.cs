// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using QRCoder;

namespace Snap.Hutao.Factory.QrCode;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IQrCodeFactory))]
internal class QrCodeFactory : IQrCodeFactory
{
    public byte[] CreateByteArr(string source)
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
