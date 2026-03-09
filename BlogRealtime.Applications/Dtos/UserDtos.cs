namespace BlogRealtime.Domain.Dtos;

public record UserDto(Guid Id, string Name, string Email);

public record UserLoginDto(string Email, string Password);

public record AuthResponseDto(string Token, UserDto User);