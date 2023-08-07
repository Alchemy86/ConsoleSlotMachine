using Microsoft.Extensions.Logging;
using SlotMachine.Application.Interfaces;
using SlotMachine.Domain.Models;

namespace SlotMachine.Domain.Services;

public class Game
{
    private readonly ISlotMachine _slotMachine;
    private readonly ILogger<Game> _logger;
    private Player _player { get; }

    public Game(ISlotMachine slotMachine, ILogger<Game> logger)
    {
        _slotMachine = slotMachine;
        _logger = logger;
        _player = new Player();
    }

    public void Play()
    {
        _logger.LogInformation(@"
_________                            .__             _________.__          __          
\_   ___ \  ____   ____   __________ |  |   ____    /   _____/|  |   _____/  |_  ______
/    \  \/ /  _ \ /    \ /  ___/  _ \|  | _/ __ \   \_____  \ |  |  /  _ \   __\/  ___/
\     \___(  <_> )   |  \\___ (  <_> )  |_\  ___/   /        \|  |_(  <_> )  |  \___ \ 
 \______  /\____/|___|  /____  >____/|____/\___  > /_______  /|____/\____/|__| /____  >
        \/            \/     \/                \/          \/                       \/ 
");
        _logger.LogInformation("Please enter your deposit amount:");

        while (true)
        {
            if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount) && depositAmount > 0)
            {
                _player.Deposit = depositAmount;
                _player.UpdateBalance(depositAmount);
                break;
            }

            _logger.LogInformation("Invalid input! Please enter a valid positive deposit amount:");
        }

        while (_player.Balance > 0)
        {
            _logger.LogInformation("Please enter your stake amount:");

            while (true)
            {
                if (decimal.TryParse(Console.ReadLine(), out decimal stakeAmount) && stakeAmount > 0 && stakeAmount <= _player.Balance)
                {
                    // Deduct the stake amount from the deposit before the spin
                    _player.UpdateBalance(-stakeAmount);

                    _slotMachine.Spin();
                    _slotMachine.DisplayResults();

                    var winAmount = _slotMachine.GetWinAmount(stakeAmount);
                    _player.UpdateBalance(winAmount);

                    Console.WriteLine(winAmount > 0
                        ? $"Congratulations! You won {winAmount:C}"
                        : "Sorry, no win this time");

                    if (_player.Balance <= 0)
                    {
                        Console.WriteLine("Game over. You have run out of funds");
                    }
                    else
                    {
                        _logger.LogInformation("Your current balance is {PlayerBalance}", _player.Balance);
                        _logger.LogInformation("Do you want to play again? (Y/N)");

                        var response = Console.ReadLine()?.Trim();
                        if (response.Equals("N", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation("Thank you for playing!");
                            return;
                        }
                    }

                    break;
                }

                _logger.LogInformation("Invalid input! Please enter a valid positive stake amount (less than or equal to your deposit):");
            }
        }
    }
  }