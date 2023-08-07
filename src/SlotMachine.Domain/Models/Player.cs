namespace SlotMachine.Domain.Models;

public class Player
{
    private decimal _balance;
    public decimal Deposit { get; set; }

    public decimal Balance
    {
        get => _balance;
        private set => _balance = value;
    }

    public void UpdateBalance(decimal changeAmount)
    {
        _balance += changeAmount;
    }
}