// 열혈 C++ 문제 02-1 [참조자 기반의 Call-by-reference 구현]
#include <iostream>
using namespace std;


void SwapByRef2(int &ref1, int &ref2)
{
    int temp = ref1;
    ref1 = ref2;
    ref2 = temp;
}
void addone(int &a)
{
    a += 1;
}

void change(int &a)
{
    a = a * (-1);
}

void SwapPointer(int &num1, int &num2)
{
    int temp;
    temp = num1;
    num1 = num2;
    num2 = temp;
}
int main(void)
{
    int val1 = 10;
    int val2 = 20;
    int num1 = 5;
    int num2 = 10;
    int *ptr1 = &num1;
    int *ptr2 = &num2;
    SwapByRef2(val1,val2);
    cout << val1 << endl;
    cout << val2 << endl;
    addone(val1);
    cout << val1 << endl;
    change(val1);
    cout << val1 << endl;
    SwapPointer(*ptr1, *ptr2);
    cout << *ptr1 << endl;
    cout << *ptr2 << endl;
    return 0;
}