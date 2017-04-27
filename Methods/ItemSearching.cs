using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CallBook.Methods
{
    class ItemSearching
    {
        //Ссылка на класс
        private static ItemSearching m_Inst = new ItemSearching();
        //Safe Read/Write start
        private static object m_lock = new object();
        private static float _timer = 0.0f;
        private static float timer
        {
            get
            {
                //Создаем локальную переменную
                float tmp = 0.0f;

                //Ожидаем когда поток освободит свойство
                lock (m_lock)
                    tmp = _timer;//Если свойство свободно тогда присваеваем

                //Возвращаем
                return tmp;
            }
            set
            {
                //Ожидаем когда поток освободит свойство
                lock (m_lock)
                    _timer = value;//Если свойство свободно тогда присваеваем
            }
        }
        private static string _tempText;
        private static string tempText
        {
            get
            {
                string tmp;
                lock (m_lock)
                    tmp = _tempText;

                return tmp;
            }
            set
            {
                lock (m_lock)
                    _tempText = value;
            }
        }

        private static List<DataBaseItem> items = null;
        public static List<DataBaseItem> resultItems
        {
            get
            {
                //На всякий случай создаем локальную переменную и присваеваем
                List<DataBaseItem> result = items;
                return result;
            }
        }

        private static bool _clearOnce = false;
        private static bool clearOnce
        {
            get
            {
                bool retVal = false;

                lock (m_lock)
                    retVal = _clearOnce;

                return retVal;
            }

            set{ _clearOnce = value;}
        }
        //Safe Read/Write End
        //Обработчик задач
        private static Task _wait;
        //------------------------------------------------

        //Работает с TextBox
        public static void Searching(string text)
        {
            //Если текст не будет ограничен
            if (text.Length > 256)
                return;

            //Не большая задержка
            timer = 25;
            //Введёный текст заносим в буфер
            tempText = text; 
            //Проверяем созданна ли задача
            if (_wait == null)
            {
                //Создаем задачу и запускаем
                _wait = Wait();
                _wait.Start();
            }
        }

        private static Task Wait()
        {
            //Запускаем задачу анонимно
            return new Task(delegate
            {
                //Это наша заморозка на не некоторое время
                while (true)
                {
                    Thread.Sleep(20);
                    timer -= 1;
                    //если счётчик меньше/равно нулю
                    if (timer <= 0.0f)
                    {
                        //Запускаем поиск элементов в базе
                        GettingItems();
                        break;
                    }
                }
            });
        }

        private static async void GettingItems()
        {
            //Console.WriteLine("Search text:" + tempText);
            //Запускаем задачу и ждём пока получим элементы
            await Task.Run(() => {
                clearOnce = true;
                if (DataBase.Load(m_Inst, tempText))
                {
                    while (DataBase.SQLRequestHandlerAlive) Thread.Sleep(50);
                    while (ItemsPreloader.BufferReader())
                    {
                        ItemsPreloader.RefreshUI(ItemsPreloader.GetFromBuffer(), clearOnce);
                        clearOnce = false;
                        //});
                    }
                }
                return true;
            });
            _wait = null;
        }
    }
}
