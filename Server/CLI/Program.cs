using CLI.UI;
using RepositoryContracts;
// using InMemoryRepositories;   // old
using FileRepositories;   // new

namespace Cli;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Choose storage:
        // If you want a quick toggle from command line, pass --memory to use in-memory.
        bool useMemory = args.Contains("--memory", StringComparer.OrdinalIgnoreCase);

        IUserRepository userRepo;
        IPostRepository postRepo;
        ICommentRepository commentRepo;

        if (useMemory)
        {
            // userRepo = new UserInMemoryRepository();
            // postRepo = new PostInMemoryRepository();
            // commentRepo = new CommentInMemoryRepository();
            throw new NotImplementedException("Wire your in-memory repos here if you want the toggle.");
        }
        else
        {
            userRepo = new UserFileRepository();
            postRepo = new PostFileRepository();
            commentRepo = new CommentFileRepository();
        }

        var app = new CliApp(userRepo, postRepo, commentRepo);
        await app.RunAsync();
    }
}
