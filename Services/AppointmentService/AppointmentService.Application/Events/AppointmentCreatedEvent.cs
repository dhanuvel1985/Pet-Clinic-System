using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Events
{
    /// <summary>
    /// Published when a new appointment is created.
    /// Keep events small, stable and versioned.
    /// </summary>
    public class AppointmentCreatedEvent
    {
        /// <summary>
        /// Event schema version. Increment when you change the payload.
        /// </summary>
        public int Version { get; init; } = 1;

        /// <summary>
        /// Appointment (aggregate) id
        /// </summary>
        public Guid AppointmentId { get; init; }

        /// <summary>
        /// Pet id (FK to PetService)
        /// </summary>
        public Guid PetId { get; init; }

        /// <summary>
        /// The user who created the appointment (owner)
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        /// Preferred appointment date/time
        /// </summary>
        public DateTime PreferredDate { get; init; }

        /// <summary>
        /// Short reason/summary for the appointment
        /// </summary>
        public string Reason { get; init; } = string.Empty;

        /// <summary>
        /// UTC timestamp when the event was created
        /// </summary>
        public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Optional correlation id for tracing across services
        /// </summary>
        public string? CorrelationId { get; init; }

        /// <summary>
        /// Optional metadata bag for forward-compatible additions (avoid heavy payloads).
        /// </summary>
        public IDictionary<string, string>? Metadata { get; init; }
    }
}
