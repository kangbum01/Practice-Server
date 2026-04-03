#include "BankingCommonDecl.h"
#include "AccountHandler.h"
#include "NormalAccount.h"
#include "HighCreditAccount.h"

AccountHandler::AccountHandler() : account(0) {}

void AccountHandler::makecount(void)
{
    int bank;
    cout << "Select count" << endl;
    cout << "1. Normal 2. HighCredit" << endl;
    cout << "choice: "; cin >> bank;
    if( bank == 1) { normalcount(); }
    else { highcount(); }
}

void AccountHandler::normalcount()
{
    int id;
    char name[NAME_LEN];
    int money;
    float rating;
    cout << "[Make Account - Normal]" << endl;
    cout << "ID: ";cin >> id;
    cout << "Name: ";cin >> name;
    cout << "Money: ";cin >> money;
    cout << "Rating: ";cin >> rating;
    arrcount[account++] = new NormalAccount(id, name, money, rating/100);

}
void AccountHandler::highcount()
{
    int id;
    char name[NAME_LEN];
    int money;
    float rating;
    int rank;
    cout << "[Make Account - Normal]" << endl;
    cout << "ID: ";cin >> id;
    cout << "Name: ";cin >> name;
    cout << "Money: ";cin >> money;
    cout << "Rating: ";cin >> rating;
    rating = rating / 100;
    cout << "Rank: ";cin >> rank;
    if( rank == 1) { rating += 0.07; }
    else if(rank == 2) { rating += 0.04; }
    else { rating += 0.02; }
    arrcount[account++] = new HighAccount(id, name, money, rating);
}