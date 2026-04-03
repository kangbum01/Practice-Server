#include <iostream>
using namespace std;

class Rectangle
{
private:
    int x;
    int y;
public:
    Rectangle(int row, int cal) : x(row), y(cal)
    {}
    Rectangle(int line) : x(line), y(line)
    {}
    void ShowAreaInfo()
    {
        cout << "space: " << x * y << endl;
    }
};

class Square : public Rectangle
{
private:
    int line;
public:
    Square(int cal) : Rectangle(cal)
    {}
};

int main(void)
{
    Rectangle rec(4,3);
    rec.ShowAreaInfo();

    Square sqr(7);
    sqr.ShowAreaInfo();
    return 0;
}