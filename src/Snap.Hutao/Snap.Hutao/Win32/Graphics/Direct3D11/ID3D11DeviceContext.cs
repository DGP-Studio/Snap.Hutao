// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal unsafe struct ID3D11DeviceContext
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x6C, 0xA9, 0xBF, 0xC0, 0x89, 0xE0, 0xFB, 0x44, 0x8E, 0xAF, 0x26, 0xF8, 0x79, 0x61, 0x90, 0xDA];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    [SuppressMessage("", "SA1313")]
    public HRESULT Map(ID3D11Resource* pResource, uint Subresource, D3D11_MAP MapType, uint MapFlags, [MaybeNull] out D3D11_MAPPED_SUBRESOURCE mappedResource)
    {
        fixed (D3D11_MAPPED_SUBRESOURCE* pMappedResource = &mappedResource)
        {
            return ThisPtr->Map((ID3D11DeviceContext*)Unsafe.AsPointer(ref this), pResource, Subresource, MapType, MapFlags, pMappedResource);
        }
    }

    [SuppressMessage("", "SA1313")]
    public void Unmap(ID3D11Resource* pResource, uint Subresource)
    {
        ThisPtr->Unmap((ID3D11DeviceContext*)Unsafe.AsPointer(ref this), pResource, Subresource);
    }

    [SuppressMessage("", "SA1313")]
    public unsafe void CopySubresourceRegion(ID3D11Resource* pDstResource, uint DstSubresource, uint DstX, uint DstY, uint DstZ, ID3D11Resource* pSrcResource, uint SrcSubresource, [AllowNull] ref readonly D3D11_BOX srcBox)
    {
        fixed (D3D11_BOX* pSrcBox = &srcBox)
        {
            ThisPtr->CopySubresourceRegion((ID3D11DeviceContext*)Unsafe.AsPointer(ref this), pDstResource, DstSubresource, DstX, DstY, DstZ, pSrcResource, SrcSubresource, pSrcBox);
        }
    }

    public void CopyResource(ID3D11Resource* pDstResource, ID3D11Resource* pSrcResource)
    {
        ThisPtr->CopyResource((ID3D11DeviceContext*)Unsafe.AsPointer(ref this), pDstResource, pSrcResource);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11DeviceChild.Vftbl ID3D11DeviceChildVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> VSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> PSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11PixelShader*, ID3D11ClassInstance**, uint, void> PSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> PSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11VertexShader*, ID3D11ClassInstance**, uint, void> VSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, int, void> DrawIndexed;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, void> Draw;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Resource*, uint, D3D11_MAP, uint, D3D11_MAPPED_SUBRESOURCE*, HRESULT> Map;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Resource*, uint, void> Unmap;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> PSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11InputLayout*, void> IASetInputLayout;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, uint*, uint*, void> IASetVertexBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Buffer*, DXGI_FORMAT, uint, void> IASetIndexBuffer;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, uint, int, uint, void> DrawIndexedInstanced;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, uint, uint, void> DrawInstanced;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> GSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11GeometryShader*, ID3D11ClassInstance**, uint, void> GSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, D3D_PRIMITIVE_TOPOLOGY, void> IASetPrimitiveTopology;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> VSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> VSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Asynchronous*, void> Begin;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Asynchronous*, void> End;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Asynchronous*, void*, uint, uint, HRESULT> GetData;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Predicate*, BOOL, void> SetPredication;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> GSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> GSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, ID3D11RenderTargetView**, ID3D11DepthStencilView*, void> OMSetRenderTargets;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, ID3D11RenderTargetView**, ID3D11DepthStencilView*, uint, uint, ID3D11UnorderedAccessView**, uint*, void> OMSetRenderTargetsAndUnorderedAccessViews;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11BlendState*, float*, uint, void> OMSetBlendState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11DepthStencilState*, uint, void> OMSetDepthStencilState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, ID3D11Buffer**, uint*, void> SOSetTargets;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, void> DrawAuto;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Buffer*, uint, void> DrawIndexedInstancedIndirect;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Buffer*, uint, void> DrawInstancedIndirect;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, uint, void> Dispatch;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Buffer*, uint, void> DispatchIndirect;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11RasterizerState*, void> RSSetState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, D3D11_VIEWPORT*, void> RSSetViewports;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, RECT*, void> RSSetScissorRects;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Resource*, uint, uint, uint, uint, ID3D11Resource*, uint, D3D11_BOX*, void> CopySubresourceRegion;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Resource*, ID3D11Resource*, void> CopyResource;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Resource*, uint, D3D11_BOX*, void*, uint, uint, void> UpdateSubresource;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Buffer*, uint, ID3D11UnorderedAccessView*, void> CopyStructureCount;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11RenderTargetView*, float*, void> ClearRenderTargetView;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11UnorderedAccessView*, uint*, void> ClearUnorderedAccessViewUint;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11UnorderedAccessView*, float*, void> ClearUnorderedAccessViewFloat;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11DepthStencilView*, uint, float, byte, void> ClearDepthStencilView;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11ShaderResourceView*, void> GenerateMips;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Resource*, float, void> SetResourceMinLOD;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Resource*, float> GetResourceMinLOD;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Resource*, uint, ID3D11Resource*, uint, DXGI_FORMAT, void> ResolveSubresource;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11CommandList*, BOOL, void> ExecuteCommandList;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> HSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11HullShader*, ID3D11ClassInstance**, uint, void> HSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> HSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> HSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> DSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11DomainShader*, ID3D11ClassInstance**, uint, void> DSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> DSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> DSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> CSSetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11UnorderedAccessView**, uint*, void> CSSetUnorderedAccessViews;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11ComputeShader*, ID3D11ClassInstance**, uint, void> CSSetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> CSSetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> CSSetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> VSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> PSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11PixelShader**, ID3D11ClassInstance**, uint*, void> PSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> PSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11VertexShader**, ID3D11ClassInstance**, uint*, void> VSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> PSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11InputLayout**, void> IAGetInputLayout;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, uint*, uint*, void> IAGetVertexBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Buffer**, DXGI_FORMAT*, uint*, void> IAGetIndexBuffer;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> GSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11GeometryShader**, ID3D11ClassInstance**, uint*, void> GSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, D3D_PRIMITIVE_TOPOLOGY*, void> IAGetPrimitiveTopology;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> VSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> VSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Predicate**, BOOL*, void> GetPredication;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> GSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> GSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, ID3D11RenderTargetView**, ID3D11DepthStencilView**, void> OMGetRenderTargets;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, ID3D11RenderTargetView**, ID3D11DepthStencilView**, uint, uint, ID3D11UnorderedAccessView**, void> OMGetRenderTargetsAndUnorderedAccessViews;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11BlendState**, float*, uint*, void> OMGetBlendState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11DepthStencilState**, uint*, void> OMGetDepthStencilState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, ID3D11Buffer**, void> SOGetTargets;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11RasterizerState**, void> RSGetState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint*, D3D11_VIEWPORT*, void> RSGetViewports;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint*, RECT*, void> RSGetScissorRects;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> HSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11HullShader**, ID3D11ClassInstance**, uint*, void> HSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> HSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> HSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> DSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11DomainShader**, ID3D11ClassInstance**, uint*, void> DSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> DSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> DSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11ShaderResourceView**, void> CSGetShaderResources;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11UnorderedAccessView**, void> CSGetUnorderedAccessViews;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11ComputeShader**, ID3D11ClassInstance**, uint*, void> CSGetShader;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11SamplerState**, void> CSGetSamplers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint, uint, ID3D11Buffer**, void> CSGetConstantBuffers;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, void> ClearState;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, void> Flush;
        internal new readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, D3D11_DEVICE_CONTEXT_TYPE> GetType;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, uint> GetContextFlags;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, BOOL, ID3D11CommandList**, HRESULT> FinishCommandList;
    }
}