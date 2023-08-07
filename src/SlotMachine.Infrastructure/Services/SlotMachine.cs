using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlotMachine.Application.Interfaces;
using SlotMachine.Domain.Models;

namespace SlotMachine.Infrastructure.Services;

public class SlotMachine : ISlotMachine
{
    private readonly ILogger<SlotMachine> _logger;
    
    private readonly Random _random;

    private readonly Symbol[][] _slots;
    private readonly List<Symbol> _symbols;
    private readonly int _rows;
    private readonly int _columns;
    
    public SlotMachine(IOptions<SymbolSettings> symbolSettingsOptions, ILogger<SlotMachine> logger, int rows = 4, int columns = 3)
    {
        _logger = logger;
        _rows = rows;
        _columns = columns;
        _random = new Random();

        _symbols = new List<Symbol>();

        foreach (var symbol in symbolSettingsOptions.Value.Symbols.Select(symbolSetting => new Symbol
                 {
                     Name = symbolSetting.Name,
                     Coefficient = symbolSetting.Coefficient,
                     Probability = symbolSetting.Probability
                 }))
        {
            _symbols.Add(symbol);
        }
        
        _slots = new Symbol[rows][];
        for (var i = 0; i < rows; i++)
        {
            _slots[i] = new Symbol[columns];
        }
    }

    public void Spin()
    {
        for (var i = 0; i < _rows; i++)
        {
            for (var j = 0; j < _columns; j++)
            {
                double rand = _random.NextDouble();

                double cumulativeProbability = 0;
                foreach (var symbol in _symbols)
                {
                    cumulativeProbability += symbol.Probability;
                    if (!(rand < cumulativeProbability)) continue;
                    _slots[i][j] = symbol;
                    break;
                }
            }
        }
    }
    
    public decimal GetWinAmount(decimal stake)
    {
        var totalWin = 0m;

        // Check for wins in each row
        for (var i = 0; i < _rows; i++)
        {
            var rowSymbols = _slots[i].Select(s => s.Name).ToArray();
            var winCoefficient = CalculateWinCoefficient(rowSymbols);
            totalWin += winCoefficient * stake;
        }

        return totalWin;
    }

    private decimal CalculateWinCoefficient(string[] rowSymbols)
    {
        var uniqueSymbols = rowSymbols.Distinct().ToArray();
        switch (uniqueSymbols.Length)
        {
            case 1 when !uniqueSymbols.Contains("*"):
            {
                // Three matching symbols in a row
                var symbolName = uniqueSymbols[0];
                var symbol = _symbols.Single(s => s.Name == symbolName);
                return symbol.Coefficient * 3;
            }
            
            // Check for a wildcard match
            case 2 when uniqueSymbols.Contains("*") && !uniqueSymbols.Contains("P"):
            {
                // Two matching symbols and a wildcard in a row
                var otherSymbol = _symbols.Single(s => s.Name == uniqueSymbols.Single(s => s != "*"));
                return otherSymbol.Coefficient * 2;
            }
            default:
                // No win
                return 0m;
        }
    }

    public void DisplayResults()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Slot Results:");

        for (var i = 0; i < _rows; i++)
        {
            for (var j = 0; j < _columns; j++)
            {
                sb.Append($"{_slots[i][j].Name} ");
            }
            sb.AppendLine();
        }

        _logger.LogInformation(sb.ToString());
    }
}