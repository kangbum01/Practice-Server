/*
파일 명: Account.h
작성자: 강범석
날짜 2026-04-3
*/
#ifndef __ACCOUNT_H__
#define __ACCOUNT_H__
class Account
{
private:
    int acID;
    int balance;
    char * cusName;
public:
    Account(int id, int money, char * name);
    Account(const Account & copy);
    int GetId() const;
    virtual void AddMoney(int money);
    void MinMoney(int money);
    void GetInfo() const;
    int GetMoney() const;
    ~Account();
};

#endif