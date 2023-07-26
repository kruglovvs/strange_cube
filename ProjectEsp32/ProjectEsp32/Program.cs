using Network;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System;
using System.Text;
using Periphery;

namespace ProjectESP32
{
    public class Program
    {
        private static Queue _instructions { get; set; } = new Queue();
        private static void Win()
        {
            PeripheryController.SetDisplay(new byte[128 * 64]);
            PeripheryController.SetLuminodiodds(new byte[9 * 3]);
            Debug.WriteLine("Win!");
            PeripheryController.OpenDoor();
            PeripheryController.Vibrate(1500);
            NetworkController.Publish("/GameData", "Win");
        }
        private static void Lose()
        {
            //PeripheryController.SetDisplay(new byte[128 * 64]);
            //PeripheryController.SetLuminodiodds(new byte[9 * 3]);
            _instructions.Clear();
            Debug.WriteLine("Lose!");
            PeripheryController.OpenDoor();
            PeripheryController.Vibrate(1500);
            NetworkController.Publish("/GameData", "Lose");
        }
        private static void WriteInstructions(byte[] instructionBytes)
        {

            WaitingAction?.Dispose();
            Debug.WriteLine("WriteInstructions!");
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
            if ((_instructions == null) || (_instructions.Count == 0))
            {
                return;
            }
            if ((_instructions.Count != 0) && ((instruction == (Instruction)_instructions.Peek())))
            {
                _instructions.Dequeue();
                WaitingAction?.Dispose();
                if (_instructions.Count == 0)
                {
                    Win();
                }
                CheckNextAction();
            }
        }
        private static void CheckNextAction()
        {
            Debug.WriteLine("CheckNextAction!");
            if ((_instructions.Count == 0))
            {
                PeripheryController.ListeningAction = Instruction.Empty;
                return;
            }
            PeripheryController.ListeningAction = (Instruction)_instructions.Peek();
            WaitingAction = new Timer(new TimerCallback((e) => { Lose(); }), null, 10000, Timeout.Infinite);
        }
        private static Timer WaitingAction { get; set; }

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
            NetworkController.TurnOn();
            Debug.WriteLine("Start main");
            PeripheryController.TurnOn();
            Debug.WriteLine("PeripheryController turned on");

            NetworkController.GotMessage += (sender, e) => { Debug.WriteLine("Got message"); if (e.Topic == "/Instructions") WriteInstructions(e.Message); };
            PeripheryController.GotAction += (e) => { CheckAction(e); };

            _instructions.Enqueue(Instruction.Vibrate);
            CheckNextAction();

            NetworkController.Publish("/Instructions", new byte[] { (byte)Instruction.ButtonPress0 });
            NetworkController.Publish("/GameData", $"check working");
            NetworkController.Publish("/BootData", $"Die");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
