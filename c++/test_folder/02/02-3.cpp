#include <iostream>
using namespace std;

typedef struct __Point
{
    int xpos;
    int ypos;
} Point;

Point& PntAdder(const Point &p1, const Point &p2)
{
    Point * p3 = new Point;
    p3 -> xpos = p1.xpos + p2.xpos;
    p3 -> ypos = p1.ypos + p2.ypos;
    return *p3;
}

int main(void)
{
    Point * p1 = new Point;
    Point * p2 = new Point;
    p1->xpos = 1;
    p1->ypos = 2;
    p2->xpos = 3;
    p2->ypos = 4;

    Point &result = PntAdder(*p1, *p2);
    cout << result.xpos << " , " << result.ypos;
    delete p1;
    delete p2;
    delete &result;
    return 0;
}