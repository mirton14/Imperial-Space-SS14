using Content.Shared.FixedPoint;
using Robust.Shared.Random;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Roles;

namespace Content.Shared.Economy
{
    public sealed class BankManagerSystem : EntitySystem
    {
        [Dependency] private readonly IRobustRandom _robustRandom = default!;

        private static Dictionary<string, BankAccountComponent> _activeBankAccounts = new();
        public override void Initialize()
        {
            base.Initialize();
        }
        public bool TryGetBankAccount(string bankAccountNumber, [MaybeNullWhen(false)] out BankAccountComponent bankAccountComponent)
        {
            _activeBankAccounts.TryGetValue(bankAccountNumber, out bankAccountComponent);
            return bankAccountComponent != null ? true : false;
        }
        public BankAccountComponent? CreateNewBankAccount()
        {
            int bankAccountNumber;
            do
            {
                bankAccountNumber = _robustRandom.Next(111111, 999999);
            } while (_activeBankAccounts.ContainsKey(bankAccountNumber.ToString()));
            string bankAccountPin = GenerateBankAccountPin();
            string bankAccountNumberStr = bankAccountNumber.ToString();
            BankAccountComponent bankAccountComponent = new BankAccountComponent(bankAccountNumberStr, bankAccountPin);
            return _activeBankAccounts.TryAdd(bankAccountNumberStr, bankAccountComponent)
                ? bankAccountComponent
                : null;
        }
        private string GenerateBankAccountPin()
        {
            var pin = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                pin += _robustRandom.Next(0, 9).ToString();
            }
            return pin;
        }
        public bool TryWithdrawFromBankAccount(string bankAccountNumber, int amount)
        {
            if(!TryGetBankAccount(bankAccountNumber, out var bankAccountComponent))
                return false;
            return bankAccountComponent.TryChangeBalanceBy(-amount);
        }
        public bool TryInsertToBankAccount(string bankAccountNumber, int amount)
        {
            if (!TryGetBankAccount(bankAccountNumber, out var bankAccountComponent))
                return false;
            return bankAccountComponent.TryChangeBalanceBy(amount);
        }
        public void TryGenerateStartingBalance(BankAccountComponent bankAccountComponent, JobPrototype jobPrototype)
        {
            if (jobPrototype.MaxBankBalance > 0)
            {
                var newBalance = FixedPoint2.New(_robustRandom.Next(jobPrototype.MinBankBalance, jobPrototype.MaxBankBalance));
                bankAccountComponent.SetBalance(newBalance);
            }
        }
    }
}
