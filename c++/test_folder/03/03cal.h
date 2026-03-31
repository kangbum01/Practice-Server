#ifndef __03cal_H__
#define __03cal_H__
#include <iostream>
using namespace std;

class Printer
{
private:
    string memory;
public:
    void SetString(string ptr)
    {
        memory = ptr;
    }

    void ShowString()
    {
        cout << memory << endl;
    }
};
class Calculator
{
private:
    int count_add;
    int count_div;
    int count_min;
public:
    void Init();
    void ShowOpCount();
    float Add(float num1, float num2);
    float Div(float num1, float num2);
    float Min(float num1, float num2);
};

inline void Calculator::Init()
{
    count_add = 0;
    count_div = 0;
    count_min = 0;
}

inline float Calculator::Add(float num1, float num2)
{
    count_add++;
    return num1 + num2;
}

inline float Calculator::Div(float num1, float num2)
{
    count_div++;
    return num1 / num2;
}

inline float Calculator::Min(float num1, float num2)
{
    count_min++;
    return num1 - num2;
}

inline void Calculator::ShowOpCount()
{
    cout << "Add: " << count_add << " Min: " << count_min << " Div: " << count_div << endl;
}

#endif