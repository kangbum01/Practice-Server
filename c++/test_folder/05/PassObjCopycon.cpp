// 복사의 Call - by - value 형식
// SimpleFuncObj(SoSimple ob)를 호출할 때 ob를 obj로 초기화 시키기 때문에 복사가 진행된다.(ob(obj)
#include<iostream>
using namespace std;

class SoSimple
{
private:
    int num;
public:
    SoSimple(int n):num(n)
    {}
    SoSimple(const SoSimple& copy): num(copy.num)
    {
        cout << "Call SoSimple(const SoSimple &copy)" << endl;
    }
    void ShowData()
    {
        cout << num << endl;
    }
};

void SimpleFuncObj(SoSimple ob)
{
    ob.ShowData();
}

int main(void)
{
    SoSimple obj(7);
    cout << "Before func" << endl;
    SimpleFuncObj(obj);
    cout << "After func " << endl;
    return 0;
}