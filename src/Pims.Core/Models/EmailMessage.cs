using System;

namespace Pims.Core.Models
{
    public sealed class EmailMessage
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public string Subject { get; init; } = string.Empty;
        public DateTimeOffset ReceivedAt { get; init; }
    }
}
