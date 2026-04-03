#ifndef __Handler__
#define __Handler__
#include "Account.h"
class AccountHandler
{
private:
    Account* arrcount[100];
    int account;
public:
    AccountHandler();
    void makecount(void);
    void inputmoney(void);
    void outputmoney(void);
    void info(void);
protected:
    void normalcount(void);
    void highcount(void);
};
#endif