namespace BlogRealtime.Domain.Entity;

public class Post
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public string Image { get; private set; }
    public User Author { get; private set; }
    public Guid UserId { get; private set; }

    protected Post()
    {
    }

    public Post(string title, string body, string image, Guid authorId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Body = body;
        Image = image;
        UserId = authorId;
    }

    public void ChangeTitle(string title) { Title = title; }

    public void ChangeBody(string body) { Body = body; }

    public void ChangeImage(string image) { Image = image; }
}
