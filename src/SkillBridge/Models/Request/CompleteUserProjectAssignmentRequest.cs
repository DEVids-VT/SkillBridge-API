namespace SkillBridge.Models.Request
{
    /// <summary>
    /// Represents a request to mark a user project assignment as complete.
    /// </summary>
    public class CompleteUserProjectAssignmentRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier of the project assignment to complete.
        /// </summary>
        public Guid ProjectAssignmentId { get; set; }

        /// <summary>
        /// Gets or sets the URL of the submission repository containing the completed work.
        /// </summary>
        public string SubmissionRepositoryUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets optional notes or comments related to the submission.
        /// </summary>
        public string? SubmissionNotes { get; set; }    
    }
}