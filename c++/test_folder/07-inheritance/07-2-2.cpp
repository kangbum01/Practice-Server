#include <iostream>
#include <cstring>
using namespace std;

class Book
{
private:
    char * title;
    char * isbn;
    int price;
public:
    Book(const char * title, const char * isbn, int money): price(money)
    {
        this->title = new char[strlen(title) + 1];
        this->isbn = new char[strlen(isbn) + 1];
        strcpy(this->title , title);
        strcpy(this->isbn, isbn);
    }
    void ShowBookInfo()
    {
        cout << "Title: " << title << endl;
        cout << "ISBN: " << isbn << endl;
        cout << "Price: " << price << endl;
    }
    ~Book()
    {
        delete []title;
        delete []isbn;
    }
};

class EBook : public Book
{
private:
    char * DRMKey;
public:
    EBook(const char * title, const char * isbn, int price, const char * DKey) : Book(title, isbn, price)
    {
        this->DRMKey = new char[strlen(DKey) + 1];
        strcpy(this->DRMKey, DKey);
    }
    void ShowEBookInfo()
    {
        ShowBookInfo();
        cout << "DRMKey: " << DRMKey << endl;
    }
    ~EBook()
    {
        delete []DRMKey;
    }
};

int main(void)
{
    Book book("Good C++", "555-12345-890-0", 200000);
    book.ShowBookInfo();
    cout << endl;
    EBook ebook("Good C++ ebook", "555-12345-890-1", 10000, "fdx9w0i8kiw");
    ebook.ShowEBookInfo();
    return 0;
}