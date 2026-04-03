// Account 클래스 상속
#include <iostream>
#include <cstring>
using namespace std;
const int NAME_LEN = 20;

class Account
{
private:
    int acID;
    int balance;
    char * cusName;
public:
    Account(int id, int money, char * name): acID(id), balance(money) 
    { 
        cusName = new char[strlen(name) + 1];
        strcpy(cusName, name);
    }
    // 깊은 복사 추가
    Account(const Account& copy):acID(copy.acID),balance(copy.balance)
    {
        cusName = new char[strlen(copy.cusName) + 1];
        strcpy(cusName, copy.cusName);
    }
    int GetId() const
    {
        return this->acID;
    }
    virtual void AddMoney(int money)
    {
        balance += money;
    }
    void MinMoney(int money)
    {
        balance -= money;
    }
    void GetInfo() const
    {
        cout << "ID: " << this->acID << endl;
        cout << "Money: " << this->balance << endl;
        cout << "Name: " << this->cusName << endl;
    }
    int GetMoney() const
    {
        return balance;
    }
    ~Account()
    {
        delete []cusName;
    }
};

// 보통 예금 계좌 Account(int id, int money, char * name): acID(id), balance(money) 
class NormalAccount : public Account
{
private:
    float rate;
public:
    NormalAccount(int id, char* name, int money, float rating): Account(id, money, name)
    {
        rate = rating / 100;
    }
    virtual void AddMoney(int money)
    {
        Account::AddMoney(money);
        Account::AddMoney(money * rate);
    }
};

// 신용 신뢰 계좌
class HighCreditAccount : public Account
{
private:
    float rate;
    int rank;
public:
    HighCreditAccount(int id, char * name, int money, float rating, int ranking): Account(id, money,name),rank(ranking)
    {
        rate = rating / 100;
        if(rank == 1)
        {
            rate += 0.07;
        }
        else if(rank == 2)
        {
            rate += 0.04;
        }
        else
        {
            rate += 0.02;
        }
    }
    virtual void AddMoney(int money)
    {
        Account::AddMoney(money);
        Account::AddMoney(money * rate);
    }
};

class AccountHandler
{
private:
    Account* arrcount[100];
    int account;
public:
    AccountHandler() : account(0)
    {}
    void makecount(void)
    {
        int id;
        char name[NAME_LEN];
        int money;
        int bank;
        cout << "Select count" << endl;
        cout << "1.Normal 2. HighCredit" << endl;
        cout << "choice: "; cin >> bank;

        cout << "[Make Account]" << endl;
        cout << "ID: ";cin >> id;
        cout << endl << "Money: ";cin >> money;
        cout << endl << "Name: ";cin >> name;
        if( bank == 1)
        {
            int rate = 0;
            cout << "rating: "; cin >> rate;
            arrcount[account++] = new NormalAccount(id,name, money, rate);
        }
        else
        {
            int rate = 0;
            int rank;
            cout << "rating: "; cin >> rate;
            cout << "Rank(1toA, 2toB, 3toC): "; cin >> rank;
            arrcount[account++] = new HighCreditAccount(id, name, money,rate,rank);
        }
    }

    void inputmoney(void)
    {
        int id;
        int money;
        bool user_in = false;
        cout << "[ Input Money ]" << endl;
        cout << "Account ID: ";
        cin >> id; cout << endl;
        cout << "Money: ";
        cin >> money; cout << endl;
        try
        {
            for(int i = 0; i < account; i++)
            {
                if( arrcount[i]->GetId() == id)
                {
                    arrcount[i]->AddMoney(money);
                    cout << "Success" << endl;
                    user_in = true;
                    break;
                }
            }
        }
        catch(const exception& e)
        {
            cerr << e.what() << '\n';
        }
    }

    void outputmoney(void)
    {
        int id;
        int money;
        bool user_in = false;
        cout << "[ Output Money ]" << endl;
        cout << "Account ID: ";
        cin >> id; cout << endl;
        cout << "Money: ";
        cin >> money; cout << endl;
        try
        {
            for(int i = 0; i < account; i++)
            {
                if( arrcount[i]->GetId() == id)
                {
                    arrcount[i]->MinMoney(money);
                    cout << "Success" << endl;
                    user_in = true;
                    break;
                }
            }
        }
        catch(const exception& e)
        {
            cerr << e.what() << '\n';
        }
    }
    void info(void)
    {
        for(int i = 0; i < account; i++)
        {
            arrcount[i]->GetInfo();
        }
    }
};



// void info(void)
// {
//     int id;
//     bool find_account = false;;
//     cout << "Account ID: ";
//     cin >> id; cout << endl;
//     for(int i = 0; i < account; i++)
//     {
//         if(arrcount[i]->GetId() == id)
//         {
//             arrcount[i]->GetInfo();
//         }

//     }
// }

void ShowMenu(void)
{
    cout << "----Menu----" << endl;
    cout << "1. Make acount" << endl;
    cout << "2. input" << endl;
    cout << "3. output" << endl;
    cout << "4. Info" << endl;
    cout << "5. QUit " << endl;
};

int main(void)
{

    AccountHandler handler;
    int choice;
    while(1)
    {
        ShowMenu();
        cout << "Num: ";
        cin >> choice;
        cout << endl; 
        switch(choice)
        {
            case 1:
                handler.makecount();
                break;
            case 2:
                handler.inputmoney();
                break;
            case 3:
                handler.outputmoney();
                break;
            case 4:
                handler.info();
                break;
            case 5:
                cout << " Thank you " << endl;
                return 0;
            default:
                cout << " Plz choose the menu" << endl;
        }
    }

    return 0;
}
