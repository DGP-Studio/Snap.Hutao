// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.IO.Http.Sharding;

[DebuggerTypeProxy(typeof(HttpShardsDebugView))]
internal sealed class HttpShards : IAsyncEnumerable<HttpShards.Shard>
{
    private readonly long length;
    private readonly long minimumLength;
    private readonly AsyncReaderWriterLock readerWriterLock = new();

    private Shard? head;

    public HttpShards(long length, long mininumLength)
    {
        this.length = length;
        this.minimumLength = mininumLength;
    }

    public async IAsyncEnumerator<Shard> GetAsyncEnumerator(CancellationToken token = default)
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
            using (await readerWriterLock.WriterLockAsync().ConfigureAwait(false))
            {
                long target = (current.Position + current.End) / 2;
                Shard next = new()
                {
                    Start = target,
                    End = current.End,
                    Next = current.Next,
                    ReaderWriterLock = readerWriterLock,
                };

                current.End = target;
                current.Next = next;

                yield return next;
                current = next.Next ?? head;

                if (!UnSyncronizedCanSplit(head))
                {
                    yield break;
                }
            }
        }
    }

    private bool UnSyncronizedCanSplit(Shard test)
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
    internal sealed class Shard
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
        private readonly List<Shard> shards = [];

        public HttpShardsDebugView(HttpShards tree)
        {
            Shard? current = tree.head;
            while (current is not null)
            {
                shards.Add(current);
                current = current.Next;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<Shard> Shards { get => shards; }
    }
}