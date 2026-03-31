// const 함수가 정의된 경우 호출할 수 있는 모든 함수들은 const로 정의 되어야 한다.
// 보통 값 출력하는 함수에 적으면 될듯
#include <iostream>
using namespace std;

class FruitSeller
{
private:
    int APPLE_PRICE;
    int numOfApples;
    int myMoney;
public:
    void InitMembers(int price, int num, int money)
    {
        APPLE_PRICE = price;
        numOfApples = num;
        myMoney = money;
    }
    int SaleApples(int money)
    {
        int num = money / APPLE_PRICE;
        numOfApples -= num;
        myMoney += money;
        return num;
    }

    void ShowSalesResult() const
    {
        cout << "Current Apples: " << numOfApples << endl;
        cout << "Sales: " << myMoney << endl;
    }
};

class FruitBuyer
{
    int myMoney;    // private
    int numOfApples;  //private , 구조체였다면 public

public:
    void InitMembers(int money)
    {
        myMoney = money;
        numOfApples = 0;
    }
    void BuyApples(FruitSeller & seller, int money)
    {
        if( money > myMoney || money == 0)
        {
            cout << "Not enough money(" << myMoney << ")" << endl; 
        }
        else
        {
            numOfApples+=seller.SaleApples(money);
            myMoney -= money;
        }

    }
    void ShowBuyResult() const
    {
        cout<< " Current money: " << myMoney << endl;
        cout << " Apple: " << numOfApples << endl;
    }
};

int main(void)
{
    FruitSeller seller;
    seller.InitMembers(1000, 20, 0); // 가격: 1000, 사과 수: 20, 현재 잔액: 0
    FruitBuyer buyer;
    buyer.InitMembers(5000);        // 현재 5000원 있다.
    buyer.BuyApples(seller, 2000); // 2000원치 구매(2개)

    cout << "Saler" << endl; // 18개 남음
    seller.ShowSalesResult();
    cout<<"Buyer " << endl; // 2개 삼
    buyer.ShowBuyResult();
    return 0;
}