// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

internal abstract class GamePackageOperationReport
{
    public GamePackageOperationReportKind ReportKind { get; private set; }

    internal sealed class Update : GamePackageOperationReport
    {
        public Update(long bytesRead, bool finished)
        {
            ReportKind = GamePackageOperationReportKind.Update;
            BytesRead = bytesRead;
            Finished = finished;
        }

        public long BytesRead { get; }

        public bool Finished { get; }
    }

    internal sealed class Reset : GamePackageOperationReport
    {
        public Reset(string title)
        {
            ReportKind = GamePackageOperationReportKind.Reset;
            Title = title;
        }

        public Reset(string title, int totalBlocks, long contentLength)
        {
            ReportKind = GamePackageOperationReportKind.Reset;
            Title = title;
            TotalBlocks = totalBlocks;
            ContentLength = contentLength;
        }

        public string Title { get; set; }

        public int TotalBlocks { get; set; }

        public long ContentLength { get; set; }
    }

    internal sealed class Finish : GamePackageOperationReport
    {
        public Finish(GamePackageOperationKind kind, bool repaired = false)
        {
            ReportKind = GamePackageOperationReportKind.Finish;
            OperationKind = kind;
            Repaired = repaired;
        }

        public GamePackageOperationKind OperationKind { get; }

        public bool Repaired { get; }
    }
}