using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace CallBook.Methods
{
    class ItemsPreloader
    {
        //И так Добро пожаловать в пред загрузку bufferList из dataBase
        //private static List<DataBaseItem> bufferList = new List<DataBaseItem>();
        private static BufferList bufferList = new BufferList();
        private static object m_lock = new object();
        //Инстанс навсякий пожарный
        private static ItemsPreloader b_Inst = new ItemsPreloader();
        private static bool _abortTaskAsync = false;
        private static bool abortTaskAsync
        {
            get
            {
                bool retVal = false;

                lock (m_lock)
                    retVal = _abortTaskAsync;

                return retVal;
            }

            set
            {
                lock (m_lock)
                    _abortTaskAsync = value;
            }
        }
        //Делегат обновления
        public delegate bool UIRefresh(List<DataBaseItem> items, bool clear);
        public static UIRefresh RefreshUI { get; private set; }
        /// <summary>
        /// Если нужно можно назначить 'null'
        /// </summary>
        /// <param name="refresh"></param>
        public static void SetUIRefresh(UIRefresh refresh)
        {
            RefreshUI = refresh;
        }

        //Смотретите в буффере
        public static void AddToBuffer(DataBaseItem item)
        {
            bufferList.AddItems(item);
        }
        //Смотретите в буффере
        public static bool BufferReader()
        {
            //Если мы всё прочитали чистем buffer
            return bufferList.Reader(true);
        }
        
        //Подготавливаем элементы и возвращаем
        public static List<DataBaseItem> GetFromBuffer()
        {
            List<DataBaseItem> tempList = new List<DataBaseItem>();
            List<DataBaseItem> buffer = bufferList.Pick();
            while(tempList.Count < buffer.Count)
            {
                int index = tempList.Count;
                DataBaseItem item = new DataBaseItem();
                item.id = buffer[index].id;
                item.firstName = buffer[index].firstName;
                item.secondName = buffer[index].secondName;
                item.lastName = buffer[index].lastName;
                item.phone = buffer[index].phone;
                item.email = buffer[index].email;
                item.street = buffer[index].street;
                tempList.Add(item);
            }
            Log.addToLog("Items added:" + buffer.Count);
            return tempList;
        }

        //Спомощью этого метода обновляем Список элементов
        public static void RefreshAll()
        {
            bool clear = true;
            //Запускаем задачу
            Task.Run(delegate
            {
                if (DataBase.LoadAll(b_Inst))
                {
                    while (DataBase.SQLRequestHandlerAlive) Thread.Sleep(50);

                    while (BufferReader())
                    {
                        RefreshUI(GetFromBuffer(), clear);
                        //Запрещаем чистить список
                        clear = false;
                        Thread.Sleep(20);
                    }
                }
            });
        }
    }
}
