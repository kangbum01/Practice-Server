#include "Account.h"
#include "BankingCommonDecl.h"


Account::Account(int id, int money, char * name): acID(id), balance(money) 
{ 
    cusName = new char[strlen(name) + 1];
    strcpy(cusName, name);
}
// 깊은 복사 추가
Account::Account(const Account& copy):acID(copy.acID),balance(copy.balance)
{
    cusName = new char[strlen(copy.cusName) + 1];
    strcpy(cusName, copy.cusName);
}
int Account::GetId() const
{
    return this->acID;
}
void Account::AddMoney(int money)
{
    balance += money;
}
void Account::MinMoney(int money)
{
    balance -= money;
}
void Account::GetInfo() const
{
    cout << "ID: " << this->acID << endl;
    cout << "Name: " << this->cusName << endl;
    cout << "Money: " << this->balance << endl;
    
}
int Account::GetMoney() const
{
    return balance;
}
Account::~Account()
{
    delete []cusName;
}