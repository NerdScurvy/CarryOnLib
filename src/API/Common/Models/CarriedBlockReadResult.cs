using System;

namespace CarryOn.API.Common.Models
{
    /// <summary>
    /// Outcome of a carried-tree read. <see cref="Ok"/> means a trustworthy
    /// <see cref="CarriedBlock"/> was produced (an empty container is still Ok);
    /// <see cref="Empty"/> means the slot is genuinely vacant (no data to read);
    /// <see cref="Failed"/> means data is present but unusable and must never be
    /// treated as an empty slot, nor silently resolved to the wrong block.
    /// </summary>
    public enum CarriedBlockReadStatus
    {
        Ok,
        Empty,
        Failed
    }

    /// <summary>
    /// Result of <see cref="CarryOn.Utility.CarriedBlockTreeSerializer.DeserializeSafe"/> preserving
    /// the distinction between a genuinely empty slot and present-but-unusable data.
    /// </summary>
    public sealed class CarriedBlockReadResult
    {
        private CarriedBlockReadResult(CarriedBlock? carried, CarriedBlockReadStatus status, string? reason, Exception? error)
        {
            Carried = carried;
            Status = status;
            Reason = reason;
            Error = error;
        }

        public CarriedBlock? Carried { get; }

        public CarriedBlockReadStatus Status { get; }

        /// <summary>Short technical reason code (not a localized string).</summary>
        public string? Reason { get; }

        /// <summary>Populated when <see cref="Status"/> is <see cref="CarriedBlockReadStatus.Failed"/> due to an exception.</summary>
        public Exception? Error { get; }

        public bool IsOk => Status == CarriedBlockReadStatus.Ok;
        public bool IsEmpty => Status == CarriedBlockReadStatus.Empty;
        public bool IsFailed => Status == CarriedBlockReadStatus.Failed;

        public static CarriedBlockReadResult Ok(CarriedBlock carried)
            => new(carried, CarriedBlockReadStatus.Ok, null, null);

        public static CarriedBlockReadResult Empty(string? reason = null)
            => new(null, CarriedBlockReadStatus.Empty, reason, null);

        public static CarriedBlockReadResult Failed(string? reason, Exception? error = null)
            => new(null, CarriedBlockReadStatus.Failed, reason, error);
    }
}
