namespace BlogRealtime.Domain.Entity;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }

    // Parameterless constructor for EF Core
    protected User()
    {
    }

    public User(string name, string email, string password)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Password = password;
    }

    public bool ValidatePassword(User userToBeTested)
    {
        return Password == userToBeTested.Password;
    }

}
