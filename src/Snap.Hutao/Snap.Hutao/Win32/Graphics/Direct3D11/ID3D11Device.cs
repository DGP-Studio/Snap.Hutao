// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal unsafe struct ID3D11Device
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xDB, 0x6D, 0x6F, 0xDB, 0x77, 0xAC, 0x88, 0x4E, 0x82, 0x53, 0x81, 0x9D, 0xF9, 0xBB, 0xF1, 0x40];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public HRESULT CreateTexture2D(ref readonly D3D11_TEXTURE2D_DESC desc, [AllowNull] ref readonly D3D11_SUBRESOURCE_DATA initialData, out ObjectReference<ID3D11Texture2D.Vftbl> texture2D)
    {
        fixed (D3D11_TEXTURE2D_DESC* pDesc = &desc)
        {
            fixed (D3D11_SUBRESOURCE_DATA* pInitialData = &initialData)
            {
                ID3D11Texture2D* pTexture2D = default;
                HRESULT hr = ThisPtr->CreateTexture2D((ID3D11Device*)Unsafe.AsPointer(ref this), pDesc, pInitialData, &pTexture2D);
                texture2D = ObjectReference<ID3D11Texture2D.Vftbl>.Attach(ref Unsafe.AsRef<nint>(&pTexture2D), ID3D11Texture2D.IID);
                return hr;
            }
        }
    }

    public void GetImmediateContext(out ObjectReference<ID3D11DeviceContext.Vftbl> immediateContext)
    {
        ID3D11DeviceContext* pImmediateContext = default;
        ThisPtr->GetImmediateContext((ID3D11Device*)Unsafe.AsPointer(ref this), &pImmediateContext);
        immediateContext = ObjectReference<ID3D11DeviceContext.Vftbl>.Attach(ref Unsafe.AsRef<nint>(&pImmediateContext), ID3D11DeviceContext.IID);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_BUFFER_DESC*, D3D11_SUBRESOURCE_DATA*, ID3D11Buffer**, HRESULT> CreateBuffer;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_TEXTURE1D_DESC*, D3D11_SUBRESOURCE_DATA*, ID3D11Texture1D**, HRESULT> CreateTexture1D;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_TEXTURE2D_DESC*, D3D11_SUBRESOURCE_DATA*, ID3D11Texture2D**, HRESULT> CreateTexture2D;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_TEXTURE3D_DESC*, D3D11_SUBRESOURCE_DATA*, ID3D11Texture3D**, HRESULT> CreateTexture3D;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, ID3D11Resource*, D3D11_SHADER_RESOURCE_VIEW_DESC*, ID3D11ShaderResourceView**, HRESULT> CreateShaderResourceView;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, ID3D11Resource*, D3D11_UNORDERED_ACCESS_VIEW_DESC*, ID3D11UnorderedAccessView**, HRESULT> CreateUnorderedAccessView;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, ID3D11Resource*, D3D11_RENDER_TARGET_VIEW_DESC*, ID3D11RenderTargetView**, HRESULT> CreateRenderTargetView;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, ID3D11Resource*, D3D11_DEPTH_STENCIL_VIEW_DESC*, ID3D11DepthStencilView**, HRESULT> CreateDepthStencilView;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_INPUT_ELEMENT_DESC*, uint, void*, nuint, ID3D11InputLayout**, HRESULT> CreateInputLayout;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, void*, nuint, ID3D11ClassLinkage*, ID3D11VertexShader**, HRESULT> CreateVertexShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, void*, nuint, ID3D11ClassLinkage*, ID3D11GeometryShader**, HRESULT> CreateGeometryShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, void*, nuint, D3D11_SO_DECLARATION_ENTRY*, uint, uint*, uint, uint, ID3D11ClassLinkage*, ID3D11GeometryShader**, HRESULT> CreateGeometryShaderWithStreamOutput;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, void*, nuint, ID3D11ClassLinkage*, ID3D11PixelShader**, HRESULT> CreatePixelShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, void*, nuint, ID3D11ClassLinkage*, ID3D11HullShader**, HRESULT> CreateHullShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, void*, nuint, ID3D11ClassLinkage*, ID3D11DomainShader**, HRESULT> CreateDomainShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, void*, nuint, ID3D11ClassLinkage*, ID3D11ComputeShader**, HRESULT> CreateComputeShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, ID3D11ClassLinkage**, HRESULT> CreateClassLinkage;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_BLEND_DESC*, ID3D11BlendState**, HRESULT> CreateBlendState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_DEPTH_STENCIL_DESC*, ID3D11DepthStencilState**, HRESULT> CreateDepthStencilState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_RASTERIZER_DESC*, ID3D11RasterizerState**, HRESULT> CreateRasterizerState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_SAMPLER_DESC*, ID3D11SamplerState**, HRESULT> CreateSamplerState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_QUERY_DESC*, ID3D11Query**, HRESULT> CreateQuery;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_QUERY_DESC*, ID3D11Predicate**, HRESULT> CreatePredicate;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_COUNTER_DESC*, ID3D11Counter**, HRESULT> CreateCounter;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, uint, ID3D11DeviceContext**, HRESULT> CreateDeferredContext;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, HANDLE, Guid*, void**, HRESULT> OpenSharedResource;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, DXGI_FORMAT, uint*, HRESULT> CheckFormatSupport;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, DXGI_FORMAT, uint, uint*, HRESULT> CheckMultisampleQualityLevels;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_COUNTER_INFO*, void> CheckCounterInfo;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_COUNTER_DESC*, D3D11_COUNTER_TYPE*, uint*, PSTR, uint*, PSTR, uint*, PSTR, uint*, HRESULT> CheckCounter;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_FEATURE, void*, uint, HRESULT> CheckFeatureSupport;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, Guid*, uint*, void*, HRESULT> GetPrivateData;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, Guid*, uint, void*, HRESULT> SetPrivateData;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, Guid*, IUnknown*, HRESULT> SetPrivateDataInterface;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, D3D_FEATURE_LEVEL> GetFeatureLevel;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, uint> GetCreationFlags;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, HRESULT> GetDeviceRemovedReason;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, ID3D11DeviceContext**, void> GetImmediateContext;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, uint, HRESULT> SetExceptionMode;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Device*, uint> GetExceptionMode;
    }
}