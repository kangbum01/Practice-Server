#include "PointTemplate.h"
#include "PointTemplate.cpp" // 이렇게 정의하기 싫으면 PointTemplate.cpp에 Point의 생성자와 멤버함수의 정의를 모두 넣으면 됨
#include <iostream>
using namespace std;

int main(void)
{
    Point<int> pos1(3,4);
    pos1.ShowPosition();

    Point<double> pos2(2.4, 3.6);
    pos2.ShowPosition();

    Point<char> pos3('P', 'F');
    pos3.ShowPosition();
    return 0;
}