using System;
using System.Threading.Tasks;
using BankingSystem;
using BankingSystem.Models;

class Program
{
    private static readonly string connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=159753521;";
    private static IBankService _bankService;
    private static IAccountService _accountService;
    private static IClientService _clientService;

    private const string ManagerPassword = "managerpass";
    private const string AdminPassword = "adminpass";

    static async Task Main()
    {
        _bankService = new BankService(connectionString);
        _accountService = new AccountService(connectionString);
        _clientService = new ClientService(connectionString);

        Console.WriteLine("Добро пожаловать в банковскую систему!");

        while (true)
        {
            Console.WriteLine("Выберите роль:");
            Console.WriteLine("1 --> Клиент");
            Console.WriteLine("2 --> Менеджер");
            Console.WriteLine("3 --> Администратор");
            Console.WriteLine("4 --> Выход");
            Console.Write("Введите номер: ");

            string roleInput = Console.ReadLine();

            try
            {
                switch (roleInput)
                {
                    case "1":
                        await ConsoleClientAsync();
                        break;
                    case "2":
                        Console.Write("Введите пароль менеджера: ");
                        string managerPass = Console.ReadLine();
                        if (managerPass == ManagerPassword)
                        {
                            await ConsoleManagerAsync();
                        }
                        else
                        {
                            Console.WriteLine("Неверный пароль менеджера.");
                        }
                        break;
                    case "3":
                        Console.Write("Введите пароль администратора: ");
                        string adminPass = Console.ReadLine();
                        if (adminPass == AdminPassword)
                        {
                            await ConsoleAdminAsync();
                        }
                        else
                        {
                            Console.WriteLine("Неверный пароль администратора.");
                        }
                        break;
                    case "4":
                        Console.WriteLine("Выход из программы.");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор роли.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }

    static async Task ConsoleClientAsync()
    {
        Console.WriteLine("Режим Клиента:");
        Console.WriteLine("1 --> Зарегистрироваться");
        Console.WriteLine("2 --> Войти по ID");
        Console.Write("Введите номер: ");

        string loginChoice = Console.ReadLine();
        int clientId = -1;

        try
        {
            if (loginChoice == "1")
            {
                Console.Write("Имя: ");
                string firstName = Console.ReadLine();
                Console.Write("Фамилия: ");
                string lastName = Console.ReadLine();
                Console.Write("Возраст: ");
                int age = int.Parse(Console.ReadLine());
                Console.Write("Адрес: ");
                string address = Console.ReadLine();
                Console.Write("Серия паспорта: ");
                string passportSeries = Console.ReadLine();
                Console.Write("Номер паспорта: ");
                string passportNumber = Console.ReadLine();
                Console.Write("ID банка: ");
                int bankId = int.Parse(Console.ReadLine());

                clientId = await _clientService.RegisterClientAsync(firstName, lastName, age, address, passportSeries, passportNumber, bankId);
                if (clientId > 0)
                {
                    Console.WriteLine($"Клиент зарегистрирован. Ваш ID: {clientId}");
                }
                else
                {
                    Console.WriteLine("Ошибка при регистрации клиента.");
                    return;
                }
            }
            else if (loginChoice == "2")
            {
                Console.Write("Введите Ваш ID клиента: ");
                clientId = int.Parse(Console.ReadLine());
                var client = await _clientService.GetClientByIdAsync(clientId);
                if (client == null)
                {
                    Console.WriteLine("Клиент с таким ID не найден.");
                    return;
                }
                Console.WriteLine($"Добро пожаловать, {client.FirstName}!");
            }
            else
            {
                Console.WriteLine("Неверный выбор.");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при входе/регистрации: {ex.Message}");
            return;
        }

        if (clientId > 0)
        {
            while (true)
            {
                Console.WriteLine("\nВыберите действие клиента:");
                Console.WriteLine("1 --> Создать счет");
                Console.WriteLine("2 --> Просмотреть счета");
                Console.WriteLine("3 --> Пополнить счет");
                Console.WriteLine("4 --> Снять деньги");
                Console.WriteLine("5 --> Выход из режима клиента");
                Console.Write("Введите номер: ");

                string clientAction = Console.ReadLine();

                try
                {
                    switch (clientAction)
                    {
                        case "1":
                            Console.Write("Тип счета (debit/credit/deposit): ");
                            string accountType = Console.ReadLine();
                            Console.Write("Начальная сумма: ");
                            decimal initialAmount = decimal.Parse(Console.ReadLine());
                            await _accountService.CreateAccountAsync(clientId, accountType, initialAmount);
                            Console.WriteLine("Счет создан.");
                            break;
                        case "2":
                            var accounts = await _accountService.GetClientAccountsAsync(clientId);
                            Console.WriteLine("Ваши счета:");
                            foreach (var account in accounts)
                                Console.WriteLine($"ID: {account.AccountId}, Тип: {account.AccountType}, Баланс: {account.Balance}, Статус: {account.Status}");
                            break;
                        case "3":
                            Console.Write("Введите ID счета для пополнения: ");
                            int depositAccountId = int.Parse(Console.ReadLine());
                            Console.Write("Введите сумму для пополнения: ");
                            decimal depositAmount = decimal.Parse(Console.ReadLine());
                            await _accountService.DepositMoneyAsync(depositAccountId, depositAmount);
                            Console.WriteLine("Счет пополнен.");
                            break;
                        case "4":
                            Console.Write("Введите ID счета для снятия: ");
                            int withdrawAccountId = int.Parse(Console.ReadLine());
                            Console.Write("Введите сумму для снятия: ");
                            decimal withdrawAmount = decimal.Parse(Console.ReadLine());
                            await _accountService.WithdrawMoneyAsync(withdrawAccountId, withdrawAmount);
                            Console.WriteLine("Деньги сняты.");
                            break;
                        case "5":
                            return; // Выход из режима клиента
                        default:
                            Console.WriteLine("Неверный выбор действия.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
        }
    }

    static async Task ConsoleManagerAsync()
    {
        Console.WriteLine("\nРежим Менеджера:");
        while (true)
        {
            Console.WriteLine("\nВыберите действие менеджера:");
            Console.WriteLine("1 --> Просмотреть клиентов");
            Console.WriteLine("2 --> Просмотреть банки");
            Console.WriteLine("3 --> Добавить банк");
            Console.WriteLine("4 --> Удалить банк");
            Console.WriteLine("5 --> Выход из режима менеджера");
            Console.Write("Введите номер: ");

            string managerAction = Console.ReadLine();

            try
            {
                switch (managerAction)
                {
                    case "1":
                        var clients = await _clientService.GetAllClientsAsync();
                        Console.WriteLine("Список клиентов:");
                        foreach (var client in clients)
                            Console.WriteLine($"ID: {client.ClientId}, {client.FirstName} {client.LastName}, Возраст: {client.Age}, Банк ID: {client.BankId}");
                        break;
                    case "2":
                        var banks = await _bankService.GetAllBanksAsync();
                        Console.WriteLine("Список банков:");
                        foreach (var bank in banks)
                            Console.WriteLine($"ID: {bank.BankId}, Название: {bank.Name}, Ставка депозита: {bank.DepositRate}, Комиссия кредита: {bank.CreditFee}");
                        break;
                    case "3":
                        Console.Write("Введите название нового банка: ");
                        string bankName = Console.ReadLine();
                        Console.Write("Введите ставку депозита: ");
                        decimal depositRate = decimal.Parse(Console.ReadLine());
                        Console.Write("Введите комиссию кредита: ");
                        decimal creditFee = decimal.Parse(Console.ReadLine());
                        await _bankService.AddBankAsync(bankName, depositRate, creditFee);
                        Console.WriteLine("Банк добавлен.");
                        break;
                    case "4":
                        Console.Write("Введите ID банка для удаления: ");
                        int deleteBankId = int.Parse(Console.ReadLine());
                        bool deleted = await _bankService.DeleteBankAsync(deleteBankId);
                        if (deleted)
                        {
                            Console.WriteLine("Банк удален.");
                        }
                        else
                        {
                             Console.WriteLine("Ошибка при удалении банка или банк не найден.");
                        }
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор действия.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }

    static async Task ConsoleAdminAsync()
    {
        Console.WriteLine("\nРежим Администратора:");
        while (true)
        {
            Console.WriteLine("\nВыберите действие администратора:");
            Console.WriteLine("1 --> Просмотреть клиентов");
            Console.WriteLine("2 --> Просмотреть банки");
            Console.WriteLine("3 --> Добавить банк");
            Console.WriteLine("4 --> Удалить банк");
            Console.WriteLine("5 --> Блокировать счет");
            Console.WriteLine("6 --> Разблокировать счет");
            Console.WriteLine("7 --> Выход из режима администратора");
            Console.Write("Введите номер: ");

            string adminAction = Console.ReadLine();

            try
            {
                switch (adminAction)
                {
                    case "1":
                        var clients = await _clientService.GetAllClientsAsync();
                        Console.WriteLine("Список клиентов:");
                        foreach (var client in clients)
                            Console.WriteLine($"ID: {client.ClientId}, {client.FirstName} {client.LastName}, Возраст: {client.Age}, Банк ID: {client.BankId}");
                        break;
                    case "2":
                        var banks = await _bankService.GetAllBanksAsync();
                        Console.WriteLine("Список банков:");
                        foreach (var bank in banks)
                            Console.WriteLine($"ID: {bank.BankId}, Название: {bank.Name}, Ставка депозита: {bank.DepositRate}, Комиссия кредита: {bank.CreditFee}");
                        break;
                     case "3":
                        Console.Write("Введите название нового банка: ");
                        string bankName = Console.ReadLine();
                        Console.Write("Введите ставку депозита: ");
                        decimal depositRate = decimal.Parse(Console.ReadLine());
                        Console.Write("Введите комиссию кредита: ");
                        decimal creditFee = decimal.Parse(Console.ReadLine());
                        await _bankService.AddBankAsync(bankName, depositRate, creditFee);
                        Console.WriteLine("Банк добавлен.");
                        break;
                    case "4":
                        Console.Write("Введите ID банка для удаления: ");
                        int deleteBankId = int.Parse(Console.ReadLine());
                        bool deleted = await _bankService.DeleteBankAsync(deleteBankId);
                        if (deleted)
                        {
                            Console.WriteLine("Банк удален.");
                        }
                        else
                        {
                             Console.WriteLine("Ошибка при удалении банка или банк не найден.");
                        }
                        break;
                    case "5":
                        Console.Write("Введите ID счета для блокировки: ");
                        int blockAccountId = int.Parse(Console.ReadLine());
                        await _accountService.BlockAccountAsync(blockAccountId);
                        Console.WriteLine("Счет заблокирован.");
                        break;
                     case "6":
                        Console.Write("Введите ID счета для разблокировки: ");
                        int unblockAccountId = int.Parse(Console.ReadLine());
                        await _accountService.UnblockAccountAsync(unblockAccountId);
                        Console.WriteLine("Счет разблокирован.");
                        break;
                    case "7":
                        return; // Выход из режима администратора
                    default:
                        Console.WriteLine("Неверный выбор действия.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}



