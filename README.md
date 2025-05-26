# Банковская система: Описание слоя работы с БД

## Этап 3: Основные команды

### **BankService**
- **AddBankAsync**  
  Добавляет новый банк в таблицу `bank`.
- **DeleteBankAsync**  
  Удаляет банк из БД, если с ним не связаны клиенты или менеджеры.
- **GetBankByIdAsync**  
  Получает информацию о банке по его ID.
- **GetAllBanksAsync**  
  Возвращает список всех банков.

### **ClientService**
- **RegisterClientAsync**  
  Регистрирует нового клиента в таблице `client`.
- **GetClientByIdAsync**  
  Получает информацию о клиенте по его ID.
- **GetAllClientsAsync**  
  Возвращает список всех клиентов.

### **AccountService**
- **CreateAccountAsync**  
  Создает новый счет (дебетовый, кредитный или депозитный).
- **BlockAccountAsync**  
  Блокирует счет по ID (устанавливает статус `frozen`).
- **UnblockAccountAsync**  
  Разблокирует счет по ID (устанавливает статус `active`).
- **DepositMoneyAsync**  
  Пополняет баланс счета и создает запись о транзакции.
- **WithdrawMoneyAsync**  
  Снимает средства со счета (если достаточно баланса) и создает запись о транзакции.
- **GetAccountByIdAsync**  
  Получает информацию о счете по его ID.
- **GetClientAccountsAsync**  
  Возвращает список всех счетов клиента.

---

## Этап 4: Структура слоя работы с БД

### **Сервисы**
1. **BankService**  
   Работает с таблицей `bank`. Предоставляет методы для:
   - Добавления банков.
   - Удаления банков.
   - Получения информации о банках.

2. **ClientService**  
   Работает с таблицей `client`. Предоставляет методы для:
   - Регистрации клиентов.
   - Получения информации о клиентах.

3. **AccountService**  
   Работает с таблицами:
   - `account` (основная таблица счетов).
   - `debitAccount` (дебетовые счета).
   - `depositAccount` (депозитные счета).
   - `creditAccount` (кредитные счета).  
   Предоставляет методы для:
   - Создания счетов.
   - Блокировки/разблокировки счетов.
   - Пополнения/снятия средств.

### **Модели**
- **Bank**  
  Свойства: `BankId`, `Name`, `DepositRate`, `CreditFee`, `CreatedAt`, `UpdatedAt`.
- **Client**  
  Свойства: `ClientId`, `FirstName`, `LastName`, `Age`, `Address`, `PassportSeries`, `PassportNumber`, `BankId`, `CreatedAt`, `UpdatedAt`.
- **Account**  
  Свойства: `AccountId`, `ClientId`, `Balance`, `InterestRate`, `AccountType`, `Status`, `CreatedAt`, `UpdatedAt`.

### **Подключение к БД**
Используется библиотека **Npgsql** для работы с PostgreSQL.  
Строка подключения:
```plaintext
Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=159753521;
