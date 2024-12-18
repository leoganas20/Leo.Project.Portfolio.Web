namespace Leo.Project.Portfolio.Api.DbSet;

public class RequestEmail
{
    public int Id { get; set; }
    public string? EmailAddress { get; set; }
    public string? Body { get; set; }
    public string? Subject { get; set; }
}