using RepositoryContracts;
using Entities;
using System.Linq;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository _users;
    private readonly IPostRepository _posts;
    private readonly ICommentRepository _comments;

    public CliApp(IUserRepository users, IPostRepository posts, ICommentRepository comments)
    {
        _users = users; _posts = posts; _comments = comments;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine("\n=== Forum CLI ===");
            Console.WriteLine("1) Create user");
            Console.WriteLine("2) Create post");
            Console.WriteLine("3) Add comment");
            Console.WriteLine("4) View posts overview");
            Console.WriteLine("5) View specific post (with comments)");
            Console.WriteLine("0) Exit");
            Console.Write("Choose: ");
            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1": await CreateUserAsync(); break;
                    case "2": await CreatePostAsync(); break;
                    case "3": await AddCommentAsync(); break;
                    case "4": ShowPostsOverview(); break;
                    case "5": await ViewSpecificPostAsync(); break;
                    case "0": return;
                    default: Console.WriteLine("Unknown option."); break;
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }
    }

    private static string ReadText(string prompt) { Console.Write(prompt); return Console.ReadLine() ?? ""; }
    private static int ReadInt(string prompt) { Console.Write(prompt); return int.TryParse(Console.ReadLine(), out var n) ? n : -1; }

    private async Task CreateUserAsync()
    {
        string username = ReadText("Username: ").Trim();
        string password = ReadText("Password: ").Trim();
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username required");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password required");

        bool taken = _users.GetManyAsync().Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (taken) throw new InvalidOperationException("Username is already taken");

        var created = await _users.AddAsync(new User { Username = username, Password = password });
        Console.WriteLine($"Created user #{created.Id} ({created.Username})");
    }

    private async Task CreatePostAsync()
    {
        int userId = ReadInt("UserId: ");
        string title = ReadText("Title: ").Trim();
        string body  = ReadText("Body: ").Trim();
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Title and Body are required");

        _ = await _users.GetSingleAsync(userId); // ensure user exists
        var created = await _posts.AddAsync(new Post { Title = title, Body = body, UserId = userId });
        Console.WriteLine($"Created post #{created.Id} by user {created.UserId}");
    }

    private async Task AddCommentAsync()
    {
        int userId = ReadInt("UserId: ");
        int postId = ReadInt("PostId: ");
        string body = ReadText("Comment: ").Trim();
        if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Comment body is required");

        _ = await _users.GetSingleAsync(userId);
        _ = await _posts.GetSingleAsync(postId);

        var created = await _comments.AddAsync(new Comment { Body = body, PostId = postId, UserId = userId });
        Console.WriteLine($"Added comment #{created.Id} to post {created.PostId}");
    }

    private void ShowPostsOverview()
    {
        var posts = _posts.GetManyAsync().OrderBy(p => p.Id).ToList();
        if (!posts.Any()) { Console.WriteLine("(no posts)"); return; }
        foreach (var p in posts) Console.WriteLine($"[{p.Id}] {p.Title}");
    }

    private async Task ViewSpecificPostAsync()
    {
        int id = ReadInt("PostId: ");
        var p = await _posts.GetSingleAsync(id);
        Console.WriteLine($"\nPost #{p.Id} (user {p.UserId})");
        Console.WriteLine($"Title: {p.Title}");
        Console.WriteLine($"Body : {p.Body}");

        var comments = _comments.GetManyAsync().Where(c => c.PostId == p.Id).OrderBy(c => c.Id).ToList();
        Console.WriteLine("\nComments:");
        if (!comments.Any()) Console.WriteLine("  (none)");
        foreach (var c in comments) Console.WriteLine($"  - [{c.Id}] (user {c.UserId}) {c.Body}");
    }
}
