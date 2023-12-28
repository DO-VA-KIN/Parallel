
    //ЗАДАЧА
//9. Сгенерировать массив из 100 строк, содержащих случайные 30000 букв латинского
//алфавита.Посчитать число вхождений буквы "a" в каждой строке, используя 1, 2 и 4
//потока.Измерить работы время программы в каждом случае.

#include <iostream>
#include <vector>
#include <string>
#include <random>
#include <chrono>
#include <omp.h>

using namespace std; 
using namespace std::chrono;

int main() {
    setlocale(LC_ALL, "RUS");

    // Засекаем время до выполнения программы
    auto start = high_resolution_clock::now();

    // Создаем генератор случайных чисел
    mt19937 gen(random_device{}());
    uniform_int_distribution<int> dis(0, 25); // для генерации случайной буквы

    // Генерируем массив из 100 строк
    vector<string> lines;
    for (int i = 0; i < 100; ++i) {
        string line;
        for (int j = 0; j < 30000; ++j) {
            char randomChar = 'a' + dis(gen); // генерируем случайный символ от 'a' до 'z'
            line.push_back(randomChar); // добавляем символ к строке
        }
        lines.push_back(line);
    }

    // Подсчитываем число вхождений буквы "a" в каждой строке
    vector<int> letterCounts;
    letterCounts.reserve(100);

#pragma omp parallel num_threads(4)//в скобках указываем сколько использовать потоков
#pragma omp for
    for (int i = 0; i < 100; i++)
    {
        int count = 0;
        for (char ch : lines[i])
        {
            if (ch == 'a')
                count++;
        }
#pragma omp critical // чтобы данные не шли в разнобой - синхронизируем 
        {
            //cout << "поток " << omp_get_thread_num() << endl; //можно использовать чтобы убедиться в использованиии нескольких потоков
            letterCounts.push_back(count);
        }

    }

    // Выводим результаты
    for (int i = 0; i < 100; i++) {
        cout << "Строка " << i << ": " << letterCounts[i] << " вхождений буквы 'a'" << endl;
    }

    // Время работы программы
    auto end = high_resolution_clock::now();
    std::chrono::duration<double> duration = end - start;
    std::cout << "Время выполнения программы: " << duration.count() << " секунд" << std::endl;

    return 0;
}

//При использовании разного кол-ва потоков заметной разницы во времени нет.
//Думаю, это обуславливается слишком частой синхронизацией(critical) и крайне малыми вычислениями для многопоточности. (возможно мой проц просто очень хорош - intel core i7 9750h)
//В реальных задачах многопоточность используют для куда более длительных вычислений и все выделенные потоки для данных вычислений являются фоновыми.
//В реальных задачах синхронизация является самым тяжелым и самым затратным по времени (взаимодействие потоков)
//Результаты вычислений можно получить и без синхронизации, если придумать и использовать сортировку после.