Этап 3
Основные команды:
    (BankService)
    -AddBankAsync (Добавляет новый банк в таблицу bank) / DeleteBankAsync (Удаляет банк из БД, если с ним не связаны клиенты или менеджеры)
    -GetBankByIdAsync (Получения информации о Банке по ID)
    -GetAllBanksAsync (Список всех банков)
    (ClientService)
    -RegisterClientAsync (Регистрация нового клиента в таблице client)
    -GetClientByIdAsync (Информация о клиенте по ID)
    -GetAllClientsAsync (Список всех клиентов)
    (AccountService)
    -CreateAccountAsync (Создаёт новыё счёт (дебитовый, кредитный, депозитный))
    -BlockAccountAsync (Блокирует счёт по ID (Устанавливает статус "frozen"))
    -UnblockAccountAsync (Разблокирует счёт по ID (Устанавливает статус "active"))
    -DepositMoneyAsync (Пополняект баланс счёта и создаёт запись о транзакции)
    -WithdrawMoneyAsync (Снимает средства со счёта (если достаточно баланса) + создаёт запись о транзакции)
    -GetAccountByIdAsync (Получает информацию о счёте по ID )
    -GetClientAccountsAsync (Список всех аккаунтов клиента)
Этап 4

Srvices:
    -BankService : Работает с таблицей bank. Предоставляет методы добавления + удаления + получения информации о банках
    -ClientService : Работает с таблицей client. Предоставляет методы для регистрации + получения информации о клиентах
    -AccountService : Работает с таблицами Account (А также с ее отростками debitAccount, depositAccount, creditAccount). Предоставляет методы для создания счетов + блокировка / разблокировка + пополнение / снятие средств

Модели:
    -Bank: Содержит свойства, соответствующие полям таблицы bank (BankId, Name, DepositRate, CreditFee, CreatedAt, UpdatedAt).
    -Client: Содержит свойства, соответствующие полям таблицы client (ClientId, FirstName, LastName, Age, Address, PassportSeries, PassportNumber,           BankId, CreatedAt, UpdatedAt).
    -Account: Содержит свойства, соответствующие полям таблицы account (AccountId, ClientId, Balance, InterestRate, AccountType, Status, CreatedAt,          UpdatedAt).
    
Подключается локально через Npgsql:
    private static readonly string connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=159753521;";

Также все методы сервисов являются фсинхронными (async / await), для ускорения выполнения обращения к БД.
    
