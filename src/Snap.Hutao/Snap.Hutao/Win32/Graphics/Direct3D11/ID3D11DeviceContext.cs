// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SuppressMessage("", "SA1313")]
[SupportedOSPlatform("windows6.1")]
internal static unsafe class ID3D11DeviceContext
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x6C, 0xA9, 0xBF, 0xC0, 0x89, 0xE0, 0xFB, 0x44, 0x8E, 0xAF, 0x26, 0xF8, 0x79, 0x61, 0x90, 0xDA]);
    }

    public static HRESULT Map(this ObjectReference<Vftbl> objRef, ObjectReference<ID3D11Resource.Vftbl> resource, uint Subresource, D3D11_MAP MapType, uint MapFlags, [MaybeNull] out D3D11_MAPPED_SUBRESOURCE mappedResource)
    {
        fixed (D3D11_MAPPED_SUBRESOURCE* pMappedResource = &mappedResource)
        {
            return objRef.Vftbl.Map(objRef.ThisPtr, resource.ThisPtr, Subresource, MapType, MapFlags, pMappedResource);
        }
    }

    public static void Unmap(this ObjectReference<Vftbl> objRef, ObjectReference<ID3D11Resource.Vftbl> resource, uint Subresource)
    {
        objRef.Vftbl.Unmap(objRef.ThisPtr, resource.ThisPtr, Subresource);
    }

    public static void CopySubresourceRegion(this ObjectReference<Vftbl> objRef, ObjectReference<ID3D11Resource.Vftbl> dstResource, uint DstSubresource, uint DstX, uint DstY, uint DstZ, ObjectReference<ID3D11Resource.Vftbl> srcResource, uint SrcSubresource, [Optional] ref readonly D3D11_BOX srcBox)
    {
        fixed (D3D11_BOX* pSrcBox = &srcBox)
        {
            objRef.Vftbl.CopySubresourceRegion(objRef.ThisPtr, dstResource.ThisPtr, DstSubresource, DstX, DstY, DstZ, srcResource.ThisPtr, SrcSubresource, pSrcBox);
        }
    }

    public static void CopyResource(this ObjectReference<Vftbl> objRef, ObjectReference<ID3D11Resource.Vftbl> dstResource, ObjectReference<ID3D11Resource.Vftbl> srcResource)
    {
        objRef.Vftbl.CopyResource(objRef.ThisPtr, dstResource.ThisPtr, srcResource.ThisPtr);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11DeviceChild.Vftbl ID3D11DeviceChildVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> VSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> PSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint*, uint, void> PSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> PSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint*, uint, void> VSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, int, void> DrawIndexed;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, void> Draw;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, D3D11_MAP, uint, D3D11_MAPPED_SUBRESOURCE*, HRESULT> Map;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, void> Unmap;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> PSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, void> IASetInputLayout;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, uint*, uint*, void> IASetVertexBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, DXGI_FORMAT, uint, void> IASetIndexBuffer;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, uint, int, uint, void> DrawIndexedInstanced;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, uint, uint, void> DrawInstanced;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> GSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint*, uint, void> GSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D_PRIMITIVE_TOPOLOGY, void> IASetPrimitiveTopology;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> VSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> VSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, void> Begin;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, void> End;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, void*, uint, uint, HRESULT> GetData;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, BOOL, void> SetPredication;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> GSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> GSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint*, nint, void> OMSetRenderTargets;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint*, nint, uint, uint, nint*, uint*, void> OMSetRenderTargetsAndUnorderedAccessViews;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, float*, uint, void> OMSetBlendState;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, void> OMSetDepthStencilState;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint*, uint*, void> SOSetTargets;
        internal readonly delegate* unmanaged[Stdcall]<nint, void> DrawAuto;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, void> DrawIndexedInstancedIndirect;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, void> DrawInstancedIndirect;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, uint, void> Dispatch;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, void> DispatchIndirect;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, void> RSSetState;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, D3D11_VIEWPORT*, void> RSSetViewports;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, RECT*, void> RSSetScissorRects;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, uint, uint, uint, nint, uint, D3D11_BOX*, void> CopySubresourceRegion;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, void> CopyResource;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, D3D11_BOX*, void*, uint, uint, void> UpdateSubresource;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, nint, void> CopyStructureCount;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, float*, void> ClearRenderTargetView;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint*, void> ClearUnorderedAccessViewUint;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, float*, void> ClearUnorderedAccessViewFloat;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, float, byte, void> ClearDepthStencilView;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, void> GenerateMips;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, float, void> SetResourceMinLOD;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, float> GetResourceMinLOD;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, nint, uint, DXGI_FORMAT, void> ResolveSubresource;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, BOOL, void> ExecuteCommandList;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> HSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint*, uint, void> HSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> HSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> HSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> DSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint*, uint, void> DSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> DSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> DSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> CSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, uint*, void> CSSetUnorderedAccessViews;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint*, uint, void> CSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> CSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> CSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> VSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> PSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, nint*, uint*, void> PSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> PSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, nint*, uint*, void> VSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> PSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, void> IAGetInputLayout;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, uint*, uint*, void> IAGetVertexBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, DXGI_FORMAT*, uint*, void> IAGetIndexBuffer;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> GSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, nint*, uint*, void> GSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D_PRIMITIVE_TOPOLOGY*, void> IAGetPrimitiveTopology;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> VSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> VSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, BOOL*, void> GetPredication;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> GSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> GSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint*, nint*, void> OMGetRenderTargets;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint*, nint*, uint, uint, nint*, void> OMGetRenderTargetsAndUnorderedAccessViews;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, float*, uint*, void> OMGetBlendState;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, uint*, void> OMGetDepthStencilState;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint*, void> SOGetTargets;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, void> RSGetState;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint*, D3D11_VIEWPORT*, void> RSGetViewports;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint*, RECT*, void> RSGetScissorRects;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> HSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, nint*, uint*, void> HSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> HSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> HSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> DSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, nint*, uint*, void> DSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> DSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> DSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> CSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> CSGetUnorderedAccessViews;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, nint*, uint*, void> CSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> CSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, nint*, void> CSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<nint, void> ClearState;
        internal readonly delegate* unmanaged[Stdcall]<nint, void> Flush;
        internal new readonly delegate* unmanaged[Stdcall]<nint, D3D11_DEVICE_CONTEXT_TYPE> GetType;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint> GetContextFlags;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL, nint*, HRESULT> FinishCommandList;
    }
}