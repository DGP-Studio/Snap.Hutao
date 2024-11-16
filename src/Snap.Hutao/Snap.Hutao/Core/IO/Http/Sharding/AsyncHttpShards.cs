// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.Diagnostics;

namespace Snap.Hutao.Core.IO.Http.Sharding;

[DebuggerTypeProxy(typeof(HttpShardsDebugView))]
internal sealed class AsyncHttpShards : IAsyncEnumerable<IHttpShard>
{
    private readonly long length;
    private readonly long minimumLength;
    private readonly AsyncReaderWriterLock readerWriterLock = new();

    private Shard? head;

    public AsyncHttpShards(long length, long minimumLength)
    {
        this.length = length;
        this.minimumLength = minimumLength;
    }

    public async IAsyncEnumerator<IHttpShard> GetAsyncEnumerator(CancellationToken token = default)
    {
        head = new()
        {
            End = length,
            ReaderWriterLock = readerWriterLock,
        };

        yield return head;

        Shard? current = head;
        while (true)
        {
            Shard next;
            using (await readerWriterLock.WriterLockAsync().ConfigureAwait(false))
            {
                long target = (current.Position + current.End) / 2;

                if (target >= current.End)
                {
                    current = current.Next ?? head;
                    continue;
                }

                next = new()
                {
                    Start = target,
                    End = current.End,
                    Next = current.Next,
                    ReaderWriterLock = readerWriterLock,
                };

                current.End = target;
                current.Next = next;
            }

            yield return next;
            current = next.Next ?? head;

            using (await readerWriterLock.WriterLockAsync().ConfigureAwait(false))
            {
                if (!UnsyncronizedCanSplit(head))
                {
                    yield break;
                }
            }
        }
    }

    private bool UnsyncronizedCanSplit(Shard test)
    {
        while (test.Next is not null)
        {
            if (test.Position + minimumLength < test.End)
            {
                return true;
            }

            test = test.Next;
        }

        return false;
    }

    [DebuggerDisplay("[{Start} - {Position} - {End}]")]
    internal sealed class Shard : IHttpShard
    {
        public Shard? Next { get; set; }

        public long Start { get; init; }

        public long End { get; set; }

        public long BytesRead { get; set; }

        public long Position { get => Start + BytesRead; }

        public required AsyncReaderWriterLock ReaderWriterLock { get; init; }
    }

    internal sealed class HttpShardsDebugView
    {
        public HttpShardsDebugView(AsyncHttpShards tree)
        {
            ImmutableArray<Shard>.Builder builder = ImmutableArray.CreateBuilder<Shard>();
            Shard? current = tree.head;
            while (current is not null)
            {
                builder.Add(current);
                current = current.Next;
            }

            Shards = builder.ToImmutable();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public ImmutableArray<Shard> Shards { get; }
    }
}