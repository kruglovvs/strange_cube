using System;
using System.Diagnostics;
using System.Threading;
using System.Device.Gpio;
using Iot.Device.Button;
using nanoFramework.Hardware.Esp32;
using Iot.Device.KeyMatrix;
using nanoFramework.Networking;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System.IO;
using System.Collections;
using static nanoFramework.Hardware.Esp32.NativeMemory;
using static ProjectESP32.Program;
using System.Text;

namespace ProjectESP32
{
    public class Program
    {
        private static Queue _instructions { get; set; } = new Queue();
        private static void Win()
        {
            //Periphery.SetDisplay(new byte[128 * 64]);
            //Periphery.SetLuminodiodds(new byte[9 * 3]);
            Debug.WriteLine("Win!");
            Periphery.OpenDoor();
            Periphery.Vibrate(1500);
            Network.Publish("/GameData", "Win!");
        }
        private static void WriteInstructions(byte[] instructionBytes)
        {
            _instructions.Clear();
            for (int i = 0; i < instructionBytes.Length; ++i) 
            {
                Debug.WriteLine($"Got Instruction: {instructionBytes[i]}");
                _instructions.Enqueue(instructionBytes[i]);
            }
            Debug.WriteLine($"action next: {(Instruction)_instructions.Peek()}");
            CheckNextAction();
        }
        private static void CheckAction(Instruction instruction)
        {
            Debug.WriteLine($"Got action: {instruction}");
            if (_instructions == null)
            {
                return;
            }
            if ((_instructions.Count != 0) && (instruction == (Instruction)_instructions.Peek()))
            {
                _instructions.Dequeue();
            }
            CheckNextAction();
        }
        private static void CheckNextAction()
        {
            Periphery.ListeningButtons = false;
            Periphery.CheckingRotation = false;
            Periphery.CheckingTemperature = false;
            Periphery.CheckingAccelation = false;

            if (_instructions.Count == 0) {
                Win();
                return;
            }
            switch ((Instruction)_instructions?.Peek())
            {
                case Instruction.Rotate:
                    Periphery.CheckingRotation = true;
                    break;
                case Instruction.Accelate:
                    Periphery.CheckingRotation = true;
                    break;
                case Instruction.HeatUp:
                case Instruction.CoolDown:
                    Periphery.CheckingRotation = true;
                    break;
                case Instruction.ButtonPress0:
                case Instruction.ButtonPress1:
                case Instruction.ButtonPress2:
                case Instruction.ButtonPress3:
                case Instruction.ButtonPress4:
                case Instruction.ButtonPress5:
                case Instruction.ButtonPress6:
                case Instruction.ButtonPress7:
                case Instruction.ButtonPress8:
                    Periphery.ListeningButtons = true;
                    break;
            }
        }

        public enum Instruction : byte
        {
            Rotate = 0x00,
            Accelate = 0x01,
            HeatUp = 0x02,
            CoolDown = 0x03,
            Illuminate = 0x04,
            //Darken = 0x05,
            Vibrate = 0x06,
            Smoke = 0x07,
            ButtonPress0 = 0x08,
            ButtonPress1 = 0x09,
            ButtonPress2 = 0x0A,
            ButtonPress3 = 0x0B,
            ButtonPress4 = 0x0C,
            ButtonPress5 = 0x0D,
            ButtonPress6 = 0x0E,
            ButtonPress7 = 0x0F,
            ButtonPress8 = 0x10,
            Empty = 0xff,
        }
        public static void Main()
        {
            Periphery.TurnOn();
            Network.TurnOn();

            Network.GotInstructions = WriteInstructions;
            Periphery.Action = CheckAction;

            Network.Publish("/Instructions", new byte[] { (byte)Instruction.ButtonPress0 });
            Network.Publish("/GameData", $"check working");
            Network.Publish("/BootData", $"Die");
        }
    }
}
