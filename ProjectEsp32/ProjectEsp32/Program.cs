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

namespace ProjectESP32
{
    public class Program {

        public enum Instruction : byte {
            Rotate,
            Accelate,
            HeatUp,
            CoolDown,
            Illuminate,
            Darken,
            Vibrate,
            Smoke,
            ButtonPress,
        }
        private static Queue _instructions { get; set; } = new Queue();
        private static void Win() {
            Periphery.SetDisplay(new byte[128 * 64]);
            Periphery.SetLuminodiodds(new byte[9 * 3]);
            Periphery.OpenDoor();
            Periphery.Vibrate(1500);
            Network.Publish("/GameData", "Win!");
        }
        private static void WriteInstructions(byte[] instructionBytes) {
            _instructions.Clear();
            foreach (Instruction instruction in instructionBytes) {
                _instructions.Enqueue(instruction);
            }
        }
        public static void Main()
        {
            Periphery.TurnOn();
            Network.TurnOn();

            Network.GotInstructions = WriteInstructions;

            Periphery.Action = (e) => 
            { 
                Debug.WriteLine($"action: {e}"); 
                if (e == (Instruction)_instructions.Peek()) {
                    _instructions.Dequeue();
                }
                if (_instructions.Count == 0) {
                    Win();
                }
            };
        }
    }
}
