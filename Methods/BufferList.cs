using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBook.Methods
{
    //И так я провозился день и ночь почитал статьи понял что ОБЪЯЗАТАЛЬНО, если работаем с List или
    //ему подобным нужно ждать пока не получил все элементы перед тем как работать с ним
    //, поэтому я решил схитрить и вот что вышло
    class BufferList
    {
        //На блокировщик
        private static object b_lock = new object();

        //Групповой список содержит класс в котором находиться Основной список данных
        private List<BufferList> groupList;

        //Основной список данных
        private List<DataBaseItem> mainList;

        //Спомощь данного класса с свойствами мы будем проверять
        //Доступен ли сейчас Основной список в Групповом списке
        //по которому мы будем обращаться
        private ListCanUse parameter;

        //Максимальное кол-во элементов в Основном списке
        private readonly static int maxItem = 10;

        //Счётчики
        //Сколько прошли
        private static int proceedCount = 0;
        //Текущий индекс группы
        private static int groupIndexer = 0;
        
        //Добавлять можно до техпор пока не будет привышен лимит
        private bool AddItem(DataBaseItem item)
        {
            if (mainList.Count < maxItem)
            {
                mainList.Add(item);
                return true;
            }
            return false;
        }

        /*private bool RemoveItem(int index)
        {
            if (mainList.Count != index)
            {
                mainList.RemoveAt(index);
                return true;
            }

            return false;
        }*/

        /// <summary>
        /// Добовляет элементы в Основной список
        /// <para>Если Основной список переполнен</para>
        /// <para>Создаеться новая группа, после это</para>
        /// <para>операция повторяеться</para>
        /// </summary>
        /// <param name="item">DataBaseItem</param>
        public void AddItems(DataBaseItem item)
        {
            int index = groupList.Count - 1;
            if (!groupList[index].AddItem(item))
            {
                groupList[index].parameter.listCanUse = true;
                //Если не получилось создаем новый лист
                BufferList b = new BufferList();
                b.mainList = new List<DataBaseItem>();
                b.parameter = new ListCanUse();
                groupList.Add(b);
                //Добавляем ещё раз
                groupList[groupList.Count - 1].AddItem(item);
            }
        }

        /// <summary>
        /// Получаем элементы Группового списка из Основного списка,
        /// <para>Счётчик после каждой успешной обработки возврастает на 1,</para>
        /// <para>Pick работает до техпор пока счётчик не будет равным с,</para>
        /// <para>Кол-во элементов Группового списка.</para>
        /// </summary>
        /// <returns></returns>
        public List<DataBaseItem> Pick()
        {
            BufferList b = new BufferList();
            while (b.mainList.Count == 0 && groupIndexer < groupList.Count)
            {
                //Console.WriteLine("Stage 1");
                if (groupList[groupIndexer].parameter == null) continue;
                //Console.WriteLine("Stage 2");
                if (!groupList[groupIndexer].parameter.listCanUse) {
                    groupList[groupIndexer].parameter.AutoUnlock();
                    continue;
                }
                //Console.WriteLine("Stage 3");
                if (groupList[groupIndexer].mainList.Count == 0) continue;
                    //throw new Exception("Failed pick from buffer maybe miss item?");
                b = groupList[groupIndexer];
                //Console.WriteLine("Stage 4");
                if (b.mainList.Count != 0)
                {
                    ++groupIndexer;
                    return b.mainList;
                }
            }
            return b.mainList;
        }

        /// <summary>
        /// Reader имеет счётчик после каждого вызова
        /// <para>Значение истинно до техпор пока счётчик</para>
        /// <para>Не будет равен Групповому списку или</para>
        /// <para>Основной список Группового списка не пуст</para>
        /// </summary>
        /// <param name="clearInComplete">Очищает все элементы</param>
        /// <returns></returns>
        public bool Reader(bool clearInComplete)
        {
            if (groupList[0].mainList.Count == 0) return false;

            bool retVal = proceedCount <= groupList.Count;
            if (retVal)
                ++proceedCount;
            else
            {
                if (clearInComplete) Clear();
            }

            return retVal;
        }

        /// <summary>
        /// Ощичает наш буфер.
        /// </summary>
        /// <returns></returns>
        private bool Clear()
        {
            bool retVal = false;
            //Console.WriteLine(DateTime.Now + " buffer begin clean!");
            while (true)
            {
                //Console.WriteLine("Stage 1");
                if (groupList[0].parameter == null) continue;
                //Console.WriteLine("Stage 2");
                if (!groupList[0].parameter.listCanUse)
                {
                    groupList[0].parameter.AutoUnlock();
                    continue;
                }
                //Console.WriteLine("Stage 3");
                if (groupList[0].mainList.Count == 0) continue;

                groupList.RemoveAt(0);


                if (groupList.Count == 0)
                {
                    //Console.WriteLine(DateTime.Now+" buffer clean!");
                    proceedCount = 0;
                    groupIndexer = 0;
                    retVal = true;

                    BufferList b = this;
                    b.mainList = new List<DataBaseItem>();
                    b.parameter = new ListCanUse();
                    groupList.Add(b);
                    break;
                }
            }

            return retVal;
        }

        private bool BufferListEmpty()
        {
            return groupList.Count == 0;
        }

        //Генерируем наш конструктор
        public BufferList()
        {
            //Тут я уже немножко запутался оставил....
            if (groupList == null)
            {
                groupList = new List<BufferList>();
                BufferList b = this;
                b.mainList = new List<DataBaseItem>();
                b.parameter = new ListCanUse();
                groupList.Add(b);
            }
        }

        
        class ListCanUse
        {
            private bool _listCanUse = false;
            public bool listCanUse
            {
                get
                {
                    bool tmp = false;
                    lock (b_lock)
                        tmp = _listCanUse;

                    return tmp;
                }
                set
                {
                    lock (b_lock)
                        _listCanUse = value;
                }
            }
            
            /// <summary>
            /// Автоматически разблокирует наш элеммент
            /// <para>Если задача отключена</para>
            /// </summary>
            public void AutoUnlock()
            {
                if (!listCanUse)
                {
                    while (DataBase.SQLRequestHandlerAlive) System.Threading.Thread.Sleep(200);

                    listCanUse = true;
                }
            }
        }
    }
}
