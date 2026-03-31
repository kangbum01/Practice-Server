#include <iostream>
#include <cstring>
using namespace std;

// class에 정의 할 내용: Name, company name, tel, Position

namespace COMP_POS
{
    enum {CLERK, SENIOR, ASSIST, MANAGER};

    void ShowNameCardInfo(int pos)
    {
        switch(pos)
        {
        case CLERK:
            cout << "CLERK"<<endl;
            break;
        case SENIOR:
            cout << "SENIOR" << endl;
            break;
        case ASSIST:
            cout << "ASSIST" << endl;
            break;
        case MANAGER:
            cout << "MANAGER" << endl;
            break;
        default:
            cout << "Wrong position" << endl;
            break;
        }
    }

}

class NameCard
{
private:
    char * name;
    char * comname;
    char * tel;
    int pos;
public:
    NameCard(const char * name, const char * comname, const char * tel ,const int pos):pos(pos)
    {
        this->name = new char[strlen(name) + 1];
        this->comname = new char[strlen(comname)+1];
        this->tel = new char[strlen(tel) + 1];
        strcpy(this->name, name);
        strcpy(this->comname, comname);
        strcpy(this->tel, tel);
    }
    void ShowNameCardInfo()
    {
        cout << "Name: " << name << endl;
        cout << "Company: " << comname << endl;
        cout << "Tel: " << tel << endl;
        cout << "Pos: "; COMP_POS::ShowNameCardInfo(pos);
        cout << endl;
    }
    ~NameCard()
    {
        delete []name;
        delete []comname;
        delete []tel;
    }

};

int main(void)
{
    NameCard manClerk("Lee", "ABCEng", "010-1111-2222",COMP_POS::CLERK);
    NameCard manSenior("Hong", "ABCEng", "010-3333-4444",COMP_POS::SENIOR);
    NameCard manAssist("Kim", "ABCEng", "010-5555-6666",COMP_POS::ASSIST);
    manClerk.ShowNameCardInfo();
    manSenior.ShowNameCardInfo();
    manAssist.ShowNameCardInfo();

    return 0;
}
