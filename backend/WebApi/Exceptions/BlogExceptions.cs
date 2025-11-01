namespace WebApi.Exceptions;

public class BlogException : Exception
{
    public BlogException(string message) : base(message) { }
    public BlogException(string message, Exception innerException) : base(message, innerException) { }
}

public class UserNotFoundException : BlogException
{
    public UserNotFoundException(int userId) : base($"User with ID {userId} was not found") { }
    public UserNotFoundException(string email) : base($"User with email '{email}' was not found") { }
}

public class PostNotFoundException : BlogException
{
    public PostNotFoundException(int postId) : base($"Post with ID {postId} was not found") { }
}

public class EmailAlreadyExistsException : BlogException
{
    public EmailAlreadyExistsException(string email) : base($"User with email '{email}' already exists") { }
}

public class InvalidCredentialsException : BlogException
{
    public InvalidCredentialsException() : base("Invalid email or password") { }
}

public class UnauthorizedOperationException : BlogException
{
    public UnauthorizedOperationException(string operation) : base($"Unauthorized to perform operation: {operation}") { }
}

public class InvalidTokenException : BlogException
{
    public InvalidTokenException() : base("Invalid or expired token") { }
}

public class BadRequestException : BlogException
{
    public BadRequestException(string message) : base(message) { }
}