using Network;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System;
using System.Text;
using Periphery;
using System.IO;
using nanoFramework.Hardware.Esp32;

namespace Game {
    public enum Instruction : byte {
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
        ButtonDisplayPress0 = 0x11,
        ButtonDisplayPress1 = 0x12,
        SetVibrationMotor = 0x13,
        OpenDoor = 0x14,
        Ping = 0x70,
        Empty = 0xff,
    }
    public class Program {
        private static Queue _instructions { get; set; } = new Queue();
        private static Timer WaitingAction { get; set; }
        private static void Win() {
            WaitingAction?.Dispose();
            _instructions?.Clear();
            //PeripheryController.SetDisplay(new byte[128 * 64]);
            //PeripheryController.SetLuminodiodds(new byte[9 * 3]);
            Debug.WriteLine("Win!");
            //PeripheryController.OpenDoor();
            //PeripheryController.Vibrate();
            NetworkController.Publish("/GameData", "Win");
            CheckNextAction();
        }
        private static void Lose() {
            _instructions?.Clear();
            //CheckNextAction();
            //PeripheryController.SetDisplay(new byte[128 * 64]);
            //PeripheryController.SetLuminodiodds(new byte[9 * 3]);
            Debug.WriteLine("Lose!");
            NetworkController.Publish("/GameData", "Lose");
            CheckNextAction();
        }
        private static void CheckMessage(string topic, byte[] message) {
            Debug.WriteLine($"Check message: {topic}, {message}");
            switch (topic) {
            case "/Instructions":
                    try
                    {
                        WaitingAction?.Dispose();
                        Debug.WriteLine("WriteInstructions!");
                        _instructions.Clear();
                        for (int i = 0; i < message.Length; ++i)
                        {
                            Debug.WriteLine($"Got Instruction: {message[i]}");
                            _instructions.Enqueue(message[i]);
                        }
                        CheckNextAction();
                    } catch
                    {
                        Debug.WriteLine("Got shit instructions!!!");
                    }
                break;
            case "/BootData":
                MemoryStream stream = new MemoryStream(message);
                //stream.Flush();
                stream.Close();
                break;
            }
        }
        private static void CheckAction(Instruction instruction) {
            Debug.WriteLine($"Check action: {instruction}");
            if ((_instructions == null) || (_instructions.Count == 0)) {
                return;
            }
            switch (instruction) {
            case Instruction.ButtonDisplayPress0:
                Sleep.StartLightSleep();
                break;
            case Instruction.ButtonDisplayPress1:
                Lose();
                break;
            default:
                if (instruction == (Instruction)_instructions.Peek()) {
                    _instructions.Dequeue();
                    WaitingAction?.Dispose();
                    if (_instructions.Count == 0) {
                        Win();
                    }
                    CheckNextAction();
                }
                break;
            }
        }
        private static void CheckNextAction() {
            Debug.WriteLine("CheckNextAction!");

            if ((_instructions == null) || (_instructions.Count == 0)) {
                PeripheryController.ListeningAction = Instruction.Empty;
                return;
            }
            Debug.WriteLine($"Next actions: {(Instruction)_instructions.Peek()}");
            switch ((Instruction)_instructions.Peek()) {
            case Instruction.OpenDoor:
                PeripheryController.OpenDoor();
                _instructions.Dequeue();
                CheckNextAction();
                break;
            case Instruction.SetVibrationMotor:
                PeripheryController.Vibrate();
                _instructions.Dequeue();
                CheckNextAction();
                break;
            case Instruction.Ping:
                NetworkController.Publish("/GameData","");
                _instructions.Dequeue();
                CheckNextAction();
                break;
            default:
                PeripheryController.ListeningAction = (Instruction)_instructions.Peek();
                WaitingAction = new Timer(new TimerCallback((e) => { Lose(); }), null, 10000, Timeout.Infinite);
                break;
            }
        }

        public static void Main() {
            Debug.WriteLine("Start main");
            NetworkController.TurnOn();
            Debug.WriteLine("Network turned on");
            PeripheryController.TurnOn();
            Debug.WriteLine("PeripheryController turned on");

            NetworkController.GotMessage += (sender, e) => { CheckMessage(e.Topic, e.Message); };
            PeripheryController.GotAction += (e) => { CheckAction(e); };
            Debug.WriteLine("made events");

            //_instructions.Enqueue(Instruction.Accelate);
            //CheckNextAction();



            Thread.Sleep(Timeout.Infinite);
        }
    }
}
