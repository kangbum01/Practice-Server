#include <iostream>
using namespace std;

class Point
{
private:
    int xpos, ypos;
public:
    void Init(int x , int y)
    {
        xpos = x;
        ypos = y;
    }
    void ShowPointInfo() const
    {
        cout<<"[" << xpos << ", " << ypos << "]" << endl;
    }
};

class Ring
{
private:
    int in_radius;
    int out_radius;
    Point in_pos;
    Point out_pos;
public:
    void Init(int xpos1, int ypos1, int rad1, int xpos2, int ypos2, int rad2)
    {
        in_pos.Init(xpos1, ypos1);
        in_radius = rad1;
        out_pos.Init(xpos2, ypos2);
        out_radius = rad2;
    }
    void ShowRingInfo()
    {
        cout << "radius: " << in_radius << endl;
        in_pos.ShowPointInfo();
        cout << "radius: " << out_radius << endl;
        out_pos.ShowPointInfo();
    }
};
int main(void)
{
    Ring ring;
    ring.Init(1,1,4,2,2,9);
    ring.ShowRingInfo();
    return 0;
}