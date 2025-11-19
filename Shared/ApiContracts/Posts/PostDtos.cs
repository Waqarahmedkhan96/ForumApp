using ApiContracts.Comments;

namespace ApiContracts.Posts
{
    //  Create
    public class CreatePostDto
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
        public required int AuthorUserId { get; set; }
    }

    // Read / Details 
    public class PostDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public required int AuthorUserId { get; set; }

        // single author name for display
        public string? AuthorName { get; set; }

        // NEW: full comment list (optional)
        public List<CommentDto> Comments { get; set; } = new();
    }

    // Update
    public class UpdatePostDto
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
    }

    // List / Summary 
    public class PostSummaryDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? AuthorName { get; set; }
    }

    // Query Parameters 
    public class PostQueryParameters
    {
        public string? TitleContains { get; set; }
        public int? AuthorUserId { get; set; }
    }
}
