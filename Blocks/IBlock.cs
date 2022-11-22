namespace ScheduPayBlockchainNetCore.Blocks
{
    public interface IBlock
    {
        string LastHash { get; set; }
        string Hash { get; }
    }
}
