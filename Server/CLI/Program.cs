using InMemoryRepositories;
using RepositoryContracts;
using CLI.UI;

IUserRepository userRepo = new UserInMemoryRepository();
IPostRepository postRepo = new PostInMemoryRepository();
ICommentRepository commentRepo = new CommentInMemoryRepository();

var app = new CliApp(userRepo, postRepo, commentRepo);
await app.RunAsync();
