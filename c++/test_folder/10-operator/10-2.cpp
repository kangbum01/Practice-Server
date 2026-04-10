#include <iostream>
using namespace std;

// pos2.operator-(pos1);
class Point
{
private:
    float xpos;
    float ypos;
public:
    Point(int x = 0, int y = 0):xpos(x),ypos(y) {}
    Point operator-()
    {
        Point pos(-xpos, -ypos);
        return pos;  // 이거 *this로 하면 pos가 반환되는 것이 아닌 -붙인 객체가 반환된다. 
    }
    void ShowPosition() const
    {
        cout << "[" << xpos << ", " << ypos << "]" << endl;
    }
    friend Point operator~(const Point &ref);
};

Point operator~(const Point &ref)
{
    Point ptr(ref.ypos, ref.xpos);
    return ptr;
}
int main(void)
{
    Point pt1(3,4);
    Point pt2 = -pt1;
    Point pt3 = ~pt2;
    pt1.ShowPosition();
    pt2.ShowPosition();
    pt3.ShowPosition();
    return 0;
}