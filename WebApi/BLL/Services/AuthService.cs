using Models.Dto.Common;

public class AuthService(
    IUserRepository userRepository,
    JwtTokenService jwtTokenService)
{
    public async Task<(UserUnit user, string token)> Register(
        string username,
        string email,
        string password,
        CancellationToken token)
    {
        var existingUsername = await userRepository.GetByUsername(username, token);
        if (existingUsername != null)
            throw new InvalidOperationException("Username already exists");

        var existingEmail = await userRepository.GetByEmail(email, token);
        if (existingEmail != null)
            throw new InvalidOperationException("Email already exists");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var now = DateTimeOffset.UtcNow;

        var newUser = new V1UserDal
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = now,
            UpdatedAt = now
        };

        var createdUser = await userRepository.Create(newUser, token);
        var jwtToken = jwtTokenService.GenerateToken(createdUser.Id, createdUser.Username, createdUser.Email);

        return (MapUser(createdUser), jwtToken);
    }

    public async Task<(UserUnit user, string token)> Login(
        string username,
        string password,
        CancellationToken token)
    {
        var user = await userRepository.GetByUsername(username, token);
        if (user == null)
            throw new InvalidOperationException("Username or password is incorrect");

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isPasswordValid)
            throw new InvalidOperationException("Username or password is incorrect");

        var jwtToken = jwtTokenService.GenerateToken(user.Id, user.Username, user.Email);

        return (MapUser(user), jwtToken);
    }

    public async Task<UserUnit> GetUserById(long id, CancellationToken token)
    {
        var user = await userRepository.GetById(id, token);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        return MapUser(user);
    }

    private UserUnit MapUser(V1UserDal user)
    {
        return new UserUnit
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
