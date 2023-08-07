using SlotMachine.Domain.Models;

namespace SlotMachine.Application.Interfaces;

public interface ISlotMachine
{
    void Spin();
    decimal GetWinAmount(decimal stake);
    void DisplayResults();
}