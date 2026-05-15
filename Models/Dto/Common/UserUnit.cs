namespace Models.Dto.Common;

public class UserUnit
{
    public long Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
