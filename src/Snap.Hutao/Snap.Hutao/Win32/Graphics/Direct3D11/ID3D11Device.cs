// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal static unsafe class ID3D11Device
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xDB, 0x6D, 0x6F, 0xDB, 0x77, 0xAC, 0x88, 0x4E, 0x82, 0x53, 0x81, 0x9D, 0xF9, 0xBB, 0xF1, 0x40]);
    }

    public static HRESULT CreateTexture2D(this ObjectReference<Vftbl> objRef, ref readonly D3D11_TEXTURE2D_DESC desc, [Optional] ref readonly D3D11_SUBRESOURCE_DATA initialData, out ObjectReference<ID3D11Texture2D.Vftbl> texture2D)
    {
        fixed (D3D11_TEXTURE2D_DESC* pDesc = &desc)
        {
            fixed (D3D11_SUBRESOURCE_DATA* pInitialData = &initialData)
            {
                nint pTexture2D = default;
                HRESULT hr = objRef.Vftbl.CreateTexture2D(objRef.ThisPtr, pDesc, pInitialData, &pTexture2D);
                texture2D = ObjectReference<ID3D11Texture2D.Vftbl>.Attach(ref pTexture2D, ID3D11Texture2D.IID);
                return hr;
            }
        }
    }

    public static void GetImmediateContext(this ObjectReference<Vftbl> objRef, out ObjectReference<ID3D11DeviceContext.Vftbl> immediateContext)
    {
        nint pImmediateContext = default;
        objRef.Vftbl.GetImmediateContext(objRef.ThisPtr, &pImmediateContext);
        immediateContext = ObjectReference<ID3D11DeviceContext.Vftbl>.Attach(ref pImmediateContext, ID3D11DeviceContext.IID);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_BUFFER_DESC*, D3D11_SUBRESOURCE_DATA*, nint*, HRESULT> CreateBuffer;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_TEXTURE1D_DESC*, D3D11_SUBRESOURCE_DATA*, nint*, HRESULT> CreateTexture1D;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_TEXTURE2D_DESC*, D3D11_SUBRESOURCE_DATA*, nint*, HRESULT> CreateTexture2D;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_TEXTURE3D_DESC*, D3D11_SUBRESOURCE_DATA*, nint*, HRESULT> CreateTexture3D;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, D3D11_SHADER_RESOURCE_VIEW_DESC*, nint*, HRESULT> CreateShaderResourceView;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, D3D11_UNORDERED_ACCESS_VIEW_DESC*, nint*, HRESULT> CreateUnorderedAccessView;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, D3D11_RENDER_TARGET_VIEW_DESC*, nint*, HRESULT> CreateRenderTargetView;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, D3D11_DEPTH_STENCIL_VIEW_DESC*, nint*, HRESULT> CreateDepthStencilView;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_INPUT_ELEMENT_DESC*, uint, void*, nuint, nint*, HRESULT> CreateInputLayout;
        internal readonly delegate* unmanaged[Stdcall]<nint, void*, nuint, nint, nint*, HRESULT> CreateVertexShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, void*, nuint, nint, nint*, HRESULT> CreateGeometryShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, void*, nuint, D3D11_SO_DECLARATION_ENTRY*, uint, uint*, uint, uint, nint, nint*, HRESULT> CreateGeometryShaderWithStreamOutput;
        internal readonly delegate* unmanaged[Stdcall]<nint, void*, nuint, nint, nint*, HRESULT> CreatePixelShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, void*, nuint, nint, nint*, HRESULT> CreateHullShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, void*, nuint, nint, nint*, HRESULT> CreateDomainShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, void*, nuint, nint, nint*, HRESULT> CreateComputeShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> CreateClassLinkage;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_BLEND_DESC*, nint*, HRESULT> CreateBlendState;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_DEPTH_STENCIL_DESC*, nint*, HRESULT> CreateDepthStencilState;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_RASTERIZER_DESC*, nint*, HRESULT> CreateRasterizerState;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_SAMPLER_DESC*, nint*, HRESULT> CreateSamplerState;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_QUERY_DESC*, nint*, HRESULT> CreateQuery;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_QUERY_DESC*, nint*, HRESULT> CreatePredicate;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_COUNTER_DESC*, nint*, HRESULT> CreateCounter;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint*, HRESULT> CreateDeferredContext;
        internal readonly delegate* unmanaged[Stdcall]<nint, HANDLE, Guid*, void**, HRESULT> OpenSharedResource;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_FORMAT, uint*, HRESULT> CheckFormatSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_FORMAT, uint, uint*, HRESULT> CheckMultisampleQualityLevels;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_COUNTER_INFO*, void> CheckCounterInfo;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_COUNTER_DESC*, D3D11_COUNTER_TYPE*, uint*, PSTR, uint*, PSTR, uint*, PSTR, uint*, HRESULT> CheckCounter;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_FEATURE, void*, uint, HRESULT> CheckFeatureSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, uint*, void*, HRESULT> GetPrivateData;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, uint, void*, HRESULT> SetPrivateData;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, nint, HRESULT> SetPrivateDataInterface;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D_FEATURE_LEVEL> GetFeatureLevel;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint> GetCreationFlags;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> GetDeviceRemovedReason;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, void> GetImmediateContext;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> SetExceptionMode;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint> GetExceptionMode;
    }
}