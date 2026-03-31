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


typedef struct {
    string acID;
    int balance;
    char cusName[NAME_LEN];
} Account;

Account accArr[100];
int countArr = 0;

void makecount(void)
{
    string id;
    char name[NAME_LEN];
    int money;
    cout << "[Make Account]" << endl;
    cout << "ID: ";
    cin >> id;
    cout << endl;
    cout << "Name: ";
    cin >> name;
    cout << endl;
    cout << "Money: ";
    cin >> money;
    cout << endl;
    accArr[countArr].acID = id;
    accArr[countArr].balance = money;
    strcpy(accArr[countArr].cusName, name);
    countArr++;
}
void inputmoney(void)
{
    string id;
    int money;
    bool user_in = false;
    cout << "[ Input Money ]" << endl;
    cout << "Account ID: ";
    cin >> id; cout << endl;
    cout << "Money: ";
    cin >> money; cout << endl;
    try
    {
        for(int i = 0; i < countArr; i++)
        {
            if( accArr[i].acID == id)
            {
                accArr[i].balance += money;
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
    string id;
    int money;
    bool user_in = false;
    cout << "[ Output Money ]" << endl;
    cout << "Account ID: ";
    cin >> id; cout << endl;
    cout << "Money: ";
    cin >> money; cout << endl;
    try
    {
        for(int i = 0; i < countArr; i++)
        {
            if( accArr[i].acID == id)
            {
                accArr[i].balance -= money;
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
    string id;
    bool find_account = false;;
    cout << "Account ID: ";
    cin >> id; cout << endl;
    for(int i = 0; i < countArr; i++)
    {
        if(accArr[i].acID == id)
        {
            cout << "Name: " << accArr[i].cusName << endl;
            cout << "Balance: " << accArr[i].balance << endl;
        }

    }
}

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