#include <iostream>
using namespace std;
// cout 는 ostream 클래스의 객체
// ostream은 이름공간 std안에 선언되어 있으며, 사용을 위해서는 헤더파일 <iostream>을 포함해야 한다.

class Point
{
private:
    int xpos, ypos;
public:
    Point(int x=0, int y=0): xpos(x), ypos(y) {};
    void ShowPosition() const
    {
        cout << '[' << xpos << ", " << ypos <<']'<<endl;
    }
    friend ostream& operator<<(ostream&, const Point&);
    friend istream& operator>>(istream&, Point&);
};

ostream& operator<<(ostream& os, const Point& pos)
{
    os << '[' << pos.xpos << ", " << pos.ypos << ']' << endl;
    return os;
}
istream& operator>>(istream& is, Point& pos)
{
    is>>pos.xpos >> pos.ypos;
    return is;
}

int main(void)
{
    Point pos1;
    cout << "x,y position enter(ex: 1 2 / 2 4): ";
    cin>>pos1;
    cout << pos1;

    Point pos2;
    cout << "x,y position enter(ex: 1 2 / 2 4): ";
    cin >> pos2;
    cout << pos2;
    return 0;

}