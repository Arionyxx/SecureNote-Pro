using System;

namespace SecureNotePro.Models
{
    /// <summary>
    /// Represents a note in the application
    /// </summary>
    public class Note
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Modified { get; set; } = DateTime.Now;
        public string? Tags { get; set; }
        public bool IsFavorite { get; set; }
        public string? Category { get; set; }
    }
}
