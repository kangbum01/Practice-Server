// const 선언을 추가해보자
// GetID랑 GetInfo 부분에 const를 추가
#include <iostream>
#include <cstring>
using namespace std;
const int NAME_LEN = 20;

void makecount(void);
void inputmoney(void);
void outputmoney(void);
void info(void);
int poweroff(void);
void ShowMenu(void);

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
    void AddMoney(int money)
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
    ~Account()
    {
        delete []cusName;
    }
};

Account * arrcount[100];
int account = 0;
void makecount(void)
{
    int id;
    char name[NAME_LEN];
    int money;
    cout << "[Make Account]" << endl;
    cout << "ID: ";cin >> id;
    cout << endl << "Money: ";cin >> money;
    cout << endl << "Name: ";cin >> name;
    arrcount[account++] = new Account(id, money, name);
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
                makecount();
                break;
            case 2:
                inputmoney();
                break;
            case 3:
                outputmoney();
                break;
            case 4:
                info();
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