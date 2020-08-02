namespace Ess3.Library.Interfaces
{
    public interface IAccount
    {
        string Name { get; set; }
        string Id { get; set; }
        string AWSAccessKey { get; set; }
        string AWSSecretKey { get; set; }
    }
}
