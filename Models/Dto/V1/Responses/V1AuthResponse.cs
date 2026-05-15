using Models.Dto.Common;

namespace Models.Dto.V1.Responses;

public class V1AuthResponse
{
    public UserUnit User { get; set; }

    public string Token { get; set; }
}
