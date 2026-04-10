#include <iostream>
using namespace std;

class Point
{
private:
    int xpos, ypos;
public:
    Point(int x=0, int y=0) : xpos(x), ypos(y) {}
    void ShowPosition() const
    {
        cout << "[" << xpos << ", " << ypos << "]" << endl;
    }
    Point operator*(int times) // pos * 3은 가능 but 3 * pos는 불가능
    {
        Point pos(xpos * times, ypos*times);
        return pos;
    }
    friend Point operator*(int times, Point & ref);
};

Point operator*(int times, Point& ref) // 교환법칙 성립을 위한 함수
{
    return ref*times;
}

int main(void)
{
    Point pos(1,2);
    Point cpy;

    cpy=pos*3;
    cpy.ShowPosition();

    cpy=pos*3*2;
    cpy.ShowPosition();
    return 0;
}
