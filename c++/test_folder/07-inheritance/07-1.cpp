#include <iostream>
#include <cstring>

using namespace std;

class Car
{
private:
    int gasolineGauge;
public:
    Car() : gasolineGauge(10)
    {}
    Car(int gasoline) : gasolineGauge(gasoline)
    {}
    int GetGasGauge()
    {
        return gasolineGauge;
    }
};

class HybridCar : public Car
{
private:
    int electricGauge;
public:
    HybridCar() : electricGauge(20)
    {}
    HybridCar(int gasol, int elect) : Car(gasol), electricGauge(elect)
    {}
    int GetElecGauge()
    {
        return electricGauge;
    }
};

class HybridWaterCar : public HybridCar
{
private:
    int waterGauge;
public:
    HybridWaterCar() : waterGauge(30)
    {}
    HybridWaterCar(int gas, int elect, int water) : HybridCar(gas, elect), waterGauge(water)
    {}
    void ShowCurrentGauge()
    {
        cout << "current Gasoline: " << GetGasGauge() << endl;
        cout << "current Elec: " << GetElecGauge() << endl;
        cout << "current water: " << waterGauge << endl;
    }
};

int main(void)
{
    HybridWaterCar car1;
    HybridWaterCar car2(100,200,300);
    car1.ShowCurrentGauge();
    car2.ShowCurrentGauge();
    return 0;
}
