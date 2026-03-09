namespace BlogRealtime.Domain.Dtos;

public record PostDto(Guid Id, string title, string body, string image, AuthorDto Author);

public record AuthorDto(string Name);

public record CreatePostDto(string Title, string Body, string Image, Guid UserId);