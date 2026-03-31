#include <iostream>
#include <cstring>
#include <ctime>
#include <cstdlib>
using namespace std;

int main(void)
{
    const char* cha1 = "hi";
    const char* cha2 = "noisy";
    char str3[50];

    cout << strlen(cha2) << endl;
    cout << strlen(cha1) << endl;

    strcpy(str3, cha1);
    strcat(str3, cha2);
    cout << str3 << endl;

    if (strcmp(cha1, cha2) == 0)
    {
        cout << "same length" << endl;
    }
    else
    {
        cout << "different length" << endl;
    }


    srand(time(NULL));
    for (int i = 0; i < 3; i++)
    {
        printf("Random Number #%d: %d\n", i, rand()%100);
    }
    return 0;

}