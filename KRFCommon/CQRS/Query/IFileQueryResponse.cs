﻿namespace KRFCommon.CQRS.Query
{
    public interface IFileQueryResponse : IQueryResponse
    {
        string MimeType { get; }
        byte[] FileBytes { get; }
        string FileName { get; }
        bool HasFileName { get; }
    }
}
