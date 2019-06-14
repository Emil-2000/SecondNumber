using System;
using System.Collections.Generic;
using System.IO;
namespace Packets
{
    class Program
    {
        static void Main(string[] args)
        {
            // загружаем файл           
            List<string> Source = LoadFile("INPUT.TXT");
            if (Source.Count != 5)
            {
                Console.WriteLine("Неправильный формат файла");
                throw new Exception("Формат файла неправильный");
            }
            // получаем имя сервера
            string ServerName = getServerName(Source);
            // получаем количество отправленных пакетов
            int Send = Source.Count - 1;
            // получаем количество полученных пакетов
            int Received = ReceivedCount(Source);
            // получаем минимальное-максимальное время отклика
            int Min = 0;
            int Max = 0;
            int Average = 0;
            getMinMax(Source, ref Min, ref Max, ref Average);
            // создаем строки для записи в файл
            List<string> testdata = CreateData(ServerName, Send, Received, Min, Max, Average);
            // записываем в файл
            SaveFile("OUTPUT.TXT", testdata);
        }
        /// <summary>
        /// Возвращает макимальные и минимальные значения возвращеннных пакетах
        /// </summary>
        /// <param name="FileData"></param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        static void getMinMax(List<string> FileData, ref int Min, ref int Max, ref int Average)
        {
            // минимальное
            int _Min = 2147483647;
            // максимальное
            int _Max = 0;
            // среднее время
            int _Average = 0;
            // число корректных времен
            int CorrectCount = 0;
            for (int i = 0; i < FileData.Count; i++)
            {
                if (i == 0)
                    continue;
                if (string.Compare(FileData[i], "Time out") == 0)
                {
                    continue;
                }
                // теперь надо разобрать строку до знака равно
                string res = "";
                bool resReady = false;
                foreach (char Item in FileData[i])
                {
                    if (resReady)
                        res += Item;
                    if (Item == '=')
                    {
                        //значит надо включить флаг добавления к res
                        resReady = true;
                    }
                }
                // конвертируем res в int
                int number = 0;
                bool success = Int32.TryParse(res, out number);
                // если все ок, то сравниваем Min, Max и добавяем
                if (success)
                {
                    _Average += number;
                    CorrectCount++;
                    if (number < _Min)
                        _Min = number;
                    if (number > _Max)
                        _Max = number;
                }
            }
            if (CorrectCount > 0)
            {
                Min = _Min;
                Max = _Max;
                double AverageD = Convert.ToDouble(_Average);
                AverageD = Math.Round(AverageD / CorrectCount, MidpointRounding.AwayFromZero);
                Average = Convert.ToInt32(AverageD);
            }
        }

        /// <summary>
        /// Вычисляет количество успешно полученных пакетов
        /// </summary>
        /// <param name="FileData"></param>
        /// <returns></returns>
        static int ReceivedCount(List<string> FileData)
        {
            int result = 0;
            for (int i = 0; i < FileData.Count; i++)
            {
                if (i == 0)
                    continue;
                if (string.Compare(FileData[i], "Time out") == 0)
                {
                    continue;
                }
                result++;
            }
            return result;
        }
        /// <summary>
        /// Получает имя сервера из списка строк
        /// </summary>
        /// <param name="FileData"></param>
        /// <returns></returns>
        static string getServerName(List<string> FileData)
        {
            return FileData[0].Substring(5);
        }
        /// <summary>
        /// Загружает файл по указанному имени возвращает список строк
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <returns>Список строк</returns>
        static List<string> LoadFile(string FileName)
        {
            List<string> result = new List<string>();
            string line = "";
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                while ((line = file.ReadLine()) != null)
                {
                    result.Add(line);
                }
                file.Close();
            }
            catch
            {
                Console.WriteLine("Ошибка чтения файла!!");
                throw new Exception("Ошибка чтения файла!!");
            }

            return result;
        }
        /// <summary>
        /// Сохраняет список строк в указанный файл
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Data">Список строк</param>
        static void SaveFile(string FileName, List<string> Data)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileName, false, System.Text.Encoding.Default))
                {
                    foreach (string Item in Data)
                        sw.WriteLine(Item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка записи файла!!! " + ex.Message);
                throw new Exception("Ошибка записи файла!!");
            }
        }
        /// <summary>
        /// Создает список строк для записи в файл заданного формата
        /// </summary>
        /// <param name="ServerName">Имя сервера</param>
        /// <param name="Send">Количество отправленных пакетов</param>
        /// <param name="Received">Количество принятых покетов</param>
        /// <param name="Minimum">Минимальное время</param>
        /// <param name="Maximum">Максимальное время</param>
        /// <returns></returns>
        static List<string> CreateData(string ServerName, int Send, int Received, int Minimum, int Maximum, int Average)
        {
            // Возвращаемый список
            List<string> result = new List<string>();
            // если все пакеты утеряны
            if (Received == 0)
            {
                result.Add("Ping statistics for " + ServerName + ":");
                result.Add("Packets: Sent = 4 Received = 0 Lost = 4 (100% loss)");
            }
            else
            {
                result.Add("Ping statistics for " + ServerName + ":");
                int Lost = Send - Received;
                int Percent = ((Send - Received) * 100 / Send);
                result.Add("Packets: Sent = " + Send + " Received = " + Received + " Lost = " + Lost + " (" + Percent + "% loss)");
                result.Add("Approximate round trip times:");
                //int Average = (Maximum + Minimum) / 2;
                result.Add("Minimum = " + Minimum + " Maximum = " + Maximum + " Average = " + Average);
            }
            return result;
        }
    }
}